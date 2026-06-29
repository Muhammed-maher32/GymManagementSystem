# 🏋️ Gym Management System (ASP.NET Core MVC)

A web app for running a gym: managing **members**, **trainers**, **membership plans**, **training sessions**, and **bookings**.

This README is written as a **learning / revision source**. It explains *what* each piece is, *why* it exists, and *how a request flows* through the whole system. If you're new to layered MVC apps, read it top-to-bottom once.

---

## 1. The Big Picture (Three-Layer Architecture)

The solution is split into **3 projects**. Each one is a separate "layer" with a single responsibility. The golden rule: **a layer only talks to the layer directly below it.**

```
┌──────────────────────────────────────────────────────────┐
│  GymSystem.PL   (Presentation Layer)                       │
│  Controllers + Razor Views (the website the user sees)     │
│  "Take the HTTP request, ask the BLL to do work,           │
│   give back an HTML page."                                 │
└───────────────────────────┬──────────────────────────────┘
                            │ calls (interfaces)
                            ▼
┌──────────────────────────────────────────────────────────┐
│  GymSystem.BLL  (Business Logic Layer)                     │
│  Services + ViewModels + AutoMapper + Result pattern       │
│  "The brain. Rules live here: is the email unique?         │
│   can this member be deleted? does the trainer match       │
│   the category?"                                           │
└───────────────────────────┬──────────────────────────────┘
                            │ calls (interfaces)
                            ▼
┌──────────────────────────────────────────────────────────┐
│  GymSystem.DAL  (Data Access Layer)                        │
│  EF Core DbContext + Models + Repositories + UnitOfWork    │
│  "Talk to the SQL Server database. Save/read rows."        │
└──────────────────────────────────────────────────────────┘
```

**Why bother splitting it?** So each part can change independently. The database code doesn't know a website exists. The website doesn't know SQL exists. Business rules sit in one place instead of being scattered in controllers.

| Project | Depends on | Think of it as |
|---|---|---|
| `GymSystem.PL` | BLL, DAL | The waiter (takes orders, brings food) |
| `GymSystem.BLL` | DAL | The chef (decides how the dish is made) |
| `GymSystem.DAL` | — | The pantry (stores & fetches ingredients) |

### Tech stack
- **ASP.NET Core MVC** – the web framework
- **Entity Framework Core** – the ORM (turns C# objects ⇆ SQL rows)
- **SQL Server** – the database
- **AutoMapper** – copies data between Entities and ViewModels automatically
- **Bootstrap + custom CSS** – the look
- **Patterns:** Repository, Unit of Work, Dependency Injection, Result pattern

---

## 2. The Database Schema (What gets stored)

### Entity inheritance: `GymUser`

`Member` and `Trainer` share a lot of fields (Name, Email, Phone, Address…). Instead of repeating them, both **inherit** from an abstract base class `GymUser`.

```
        BaseEntity                 (Id, CreatedAt, UpdatedAt)  ← every entity has these
            ▲
        GymUser                    (Name, Email, Phone, DateOfBirth, Gender, Address)
        ▲       ▲
    Member     Trainer
   (+Photo)    (+Speciality)
```

> **Address** is an **owned type** (`[Owned]`). It's not its own table — its columns (`City`, `Street`, `BuildingNumber`) are folded *into* the Members and Trainers tables. It's just a clean way to group related fields in C#.

### The tables

| Entity | Purpose | Key fields |
|---|---|---|
| **Member** | A gym customer | inherits GymUser + `Photo` |
| **Trainer** | A coach | inherits GymUser + `Speciality` (enum) |
| **Plan** | A membership package | `Name`, `Price`, `DurationDays`, `IsActive` |
| **Category** | Session type (Yoga, Boxing…) | `Name` *(seeded)* |
| **Session** | A scheduled class | `Description`, `Capacity`, `EndDate`, FK→Trainer, FK→Category |
| **HealthRecord** | A member's health info | `Height`, `Weight`, `BloodType`, `Notes`, FK→Member |
| **Booking** | *Junction:* a member attending a session | `BookingDate`, `IsAttended`, FK→Member, FK→Session |
| **Membership** | *Junction:* a member subscribed to a plan | `StartDate`, `EndDate`, FK→Member, FK→Plan |

### Relationships (read these out loud)

- A **Member** has **one** HealthRecord. *(1 ─ 1)*
- A **Member** has **many** Memberships (over time), each linking to **one** Plan. *(many ─ many via Membership)*
- A **Member** has **many** Bookings, each linking to **one** Session. *(many ─ many via Booking)*
- A **Trainer** conducts **many** Sessions. *(1 ─ many)*
- A **Category** classifies **many** Sessions. *(1 ─ many)*

```
HealthRecord 1───1 Member ──many──< Membership >──many── Plan
                     │
                   many
                     │
                 Booking
                     │
                   many
                     │
                 Session >──many──1 Trainer
                     │
                   many──1 Category
```

> **Why "junction" tables (Booking, Membership)?** A member can join many sessions, and a session has many members — that's a **many-to-many** relationship. SQL can't store that directly, so we insert a middle table. The bonus: the middle table can hold *extra* facts about the link (e.g. `IsAttended`, `BookingDate`).

---

## 3. ⭐ The One Trick That Confuses Everyone: `CreatedAt` reused as date columns

`BaseEntity` gives every entity a `CreatedAt`. The project **renames that column** per-entity instead of adding new date fields:

| Entity | C# property | DB column name | Set by |
|---|---|---|---|
| Member | `CreatedAt` | **`JoinDate`** | SQL `GetDate()` default |
| Trainer | `CreatedAt` | **`HireDate`** | SQL `GetDate()` default |
| Session | `CreatedAt` | **`StartDate`** | mapped from the form's StartDate |

You can see this in the EF configurations, e.g. `SessionConfiguration.cs`:

```csharp
builder.Property(x => x.CreatedAt)
       .HasColumnName("StartDate")
       .HasDefaultValueSql("GetDate()");
```

And in `MappingProfile.cs`, when a session is created the form's `StartDate` is written into `CreatedAt`:

```csharp
CreateMap<CreateSessionViewModel, Session>()
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(s => s.StartDate));
```

> 💡 **So when you see `session.CreatedAt`, mentally read it as `StartDate`.** Same for `member.CreatedAt` = JoinDate. This saves adding extra properties, but it's the #1 thing to remember when reading the code.

---

## 4. Database Rules enforced in SQL (not just C#)

Some rules are baked into the database itself via **check constraints** (in the `*Configuration.cs` files), so bad data can never be saved even if the C# code has a bug:

- **GymUser:** Email must look like an email; Phone must start with `010 / 011 / 012 / 015` (Egyptian numbers); Email & Phone are **unique** (indexes).
- **Session:** `Capacity BETWEEN 1 AND 25`; `StartDate < EndDate`.
- **Membership / Booking:** `StartDate` / `BookingDate` default to `GetDate()` (now).

---

## 5. The DAL building blocks (Repository + Unit of Work)

### `IGenericRepository<T>` — generic CRUD
One reusable repository works for *any* entity (`Member`, `Plan`, `Session`…). It hides EF Core behind simple methods:

```csharp
Task<TEntity?> GetByIdAsync(int id, CancellationToken ct);
Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct);
void Add(TEntity entity);
void Update(TEntity entity);
void Delete(TEntity entity);
Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken ct);
Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken ct);
```

> **Why a repository?** So services say `repo.GetByIdAsync(5)` instead of writing raw EF queries everywhere. If we ever swap the ORM, only the repository changes.

### `ISessionRepository` — a *specialized* repository
Some queries are too specific for the generic one (e.g. "get sessions **with** their Trainer & Category loaded", or "count booked slots"). Those live in a dedicated `SessionRepository` that **inherits** the generic one and adds extra methods.

### `UnitOfWork` — one transaction, many repositories
`IUnitOfWork` hands out repositories **and** owns `SaveChangesAsync()`.

```csharp
var memberRepo = _unitOfWork.GetRepository<Member>();
memberRepo.Add(member);              // staged in memory, NOT saved yet
await _unitOfWork.SaveChangesAsync(ct);  // ← everything committed together here
```

> **Why?** `Add/Update/Delete` only stage changes. Nothing hits the DB until `SaveChangesAsync`. That means several operations commit **together** (all-or-nothing). One `DbContext` is shared across all repositories in a request, so they're part of the same transaction.

---

## 6. The BLL building blocks

### ViewModels (not Entities) cross the boundary
Controllers never receive raw database entities. They use **ViewModels** — shapes tailored to a screen:
- `CreateMemberViewModel` – exactly the fields the create form needs (+ validation attributes)
- `MemberDetailsViewModel` – a flattened view (e.g. `Address` becomes one string `"15-Makram Ebeid-Cairo"`)
- `PlanViewModel`, `SessionViewModel`, etc.

> **Why?** Security + clarity. You don't want to expose internal DB fields or accept them blindly from a form. ViewModels also let you reshape data (flatten the address, add `AvailableSlots`, etc.).

### AutoMapper does the copying
`MappingProfile.cs` defines how Entity ⇆ ViewModel. Instead of hand-writing `vm.Name = entity.Name; vm.Email = entity.Email; …`, AutoMapper copies matching properties and you only configure the *special* cases:

```csharp
CreateMap<Member, MemberDetailsViewModel>()
    .ForMember(d => d.Address,
               o => o.MapFrom(m => $"{m.Address.BuildingNumber}-{m.Address.Street}-{m.Address.City}"));
```

### The Result pattern (instead of throwing exceptions)
Services return a `Result` / `Result<T>` object that says *succeeded or not, and why*:

```csharp
public sealed record Result(bool Success, string? Error = null, ResultKind Kind = ResultKind.Ok);
// ResultKind: Ok | NotFound | Conflict | ValidationFailed | Forbidden

Result.Ok();
Result.Fail("Email already in use.");
Result.NotFound("Member not found.");
Result<MemberViewModel>.Ok(vm);
```

> **Why not just throw exceptions?** Business outcomes like "email already taken" are *expected*, not crashes. Returning a `Result` keeps control flow clean: the controller checks `result.Success` and shows an error message instead of try/catch everywhere.

---

## 7. 🔄 Full Request Flow — Creating a Member (step by step)

This is the whole journey of one click. Follow the arrows.

```
[User fills "Create Member" form, clicks Submit]
        │  HTTP POST /Members/CreateMember
        ▼
┌─ PL: MembersController.CreateMember(CreateMemberViewModel model) ─┐
│  1. ModelState.IsValid?  (checks [Required], phone format, …)     │
│     └─ invalid → redisplay the form with errors                   │
│  2. await _memberService.CreateMemberAsync(model, ct)             │
└───────────────────────────────┬───────────────────────────────────┘
                                │
                                ▼
┌─ BLL: MemberService.CreateMemberAsync ────────────────────────────┐
│  3. emailExists = repo.AnyAsync(m => m.Email == model.Email)      │  ← business rule
│  4. phoneExists = repo.AnyAsync(m => m.Phone == model.Phone)      │  ← business rule
│     └─ if either → return Result.Fail("…already in use")          │
│  5. member = _mapper.Map<Member>(model)   (ViewModel → Entity)    │
│  6. repo.Add(member)                       (staged)               │
│  7. rows = _unitOfWork.SaveChangesAsync(ct) (COMMIT)              │
│  8. return rows > 0 ? Result.Ok() : Result.Fail("Try again.")    │
└───────────────────────────────┬───────────────────────────────────┘
                                │
                                ▼  (EF Core builds INSERT, SQL Server stores the row)
                          [ SQL Server ]
                                │
                                ▼  Result travels back up
┌─ PL: back in the controller ─────────────────────────────────────┐
│  9. result.Success?                                               │
│     ├─ yes → TempData["SuccessMessage"]; redirect to Index        │
│     └─ no  → ModelState error; redisplay Create form              │
└──────────────────────────────────────────────────────────────────┘
        │
        ▼
[Browser shows the member list with a green success alert]
```

**Read flow (e.g. `/Members/Index`) is the same, just reversed and simpler:** controller → service `GetAllMembersAsync` → repo `GetAllAsync` → entities → AutoMapper → `MemberViewModel`s → `View(model)` → HTML table.

---

## 8. Where the business rules live (quick map)

| Rule (from the spec) | Implemented in |
|---|---|
| Email & phone unique/valid | `GymUserConfiguration` (DB) + `MemberService` (check) |
| Can't delete member with **future bookings** | `MemberService.RemoveMemberAsync` |
| Session: end after start, start in future | `SessionService.CreateSessionAsync` |
| Session: trainer's speciality must match the category | `SessionService.CreateSessionAsync` |
| Capacity 1–25 | `SessionConfiguration` (DB check constraint) |
| Plan soft-delete (`IsActive`) | `Plan.IsActive` |
| Member's active membership lookup | `MemberService.GetMemberDetailsByIdAsync` |

---

## 9. App startup & data seeding

`Program.cs` wires everything up with **Dependency Injection** — it registers each interface → concrete class so the framework can inject them into constructors:

```csharp
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
builder.Services.AddDbContext<GymDbContext>(o => o.UseSqlServer(/* DefaultConnection */));
```

> **`Scoped`** = one instance per HTTP request. That's why all repositories in a request share the same `DbContext` (same transaction).

On startup, `ProgramExtensions.MigrateAndSeedAsync()`:
1. Applies any **pending EF migrations** (creates/updates the DB).
2. Seeds **Plans** and **Trainers** from JSON files in `wwwroot/Files/` (`plans.json`, `trainers.json`) — but only if those tables are empty.

---

## 10. Project layout (where to find things)

```
GymSystem.DAL/
 ├─ Models/                 ← entities (Member, Session, Booking…) + Enums
 ├─ Data/
 │   ├─ AppDbContexts/      ← GymDbContext
 │   ├─ Configurations/     ← EF rules per entity (column names, constraints)
 │   └─ DataSeed/           ← GymDataSeeding (reads JSON)
 ├─ Repositories/           ← Generic + Session repos, UnitOfWork (+ interfaces)
 └─ Migrations/             ← auto-generated DB scripts

GymSystem.BLL/
 ├─ Services/
 │   ├─ Contracts/          ← IMemberService, ISessionService, IPlanService
 │   ├─ Classes/            ← the implementations (the rules)
 │   └─ Common/             ← Result / Result<T> / ResultKind
 ├─ ViewModels/             ← per-feature view models
 └─ MappingProfile.cs       ← AutoMapper config

GymSystem.PL/
 ├─ Controllers/            ← Home, Members, Sessions, Plans
 ├─ Views/                  ← Razor (.cshtml) pages, grouped by controller
 ├─ wwwroot/                ← css, js, images, lib, Files/ (seed json)
 ├─ Program.cs              ← startup + DI
 └─ ProgramExtensions.cs    ← migrate + seed on boot
```

---

## 11. How to run

1. Set the `DefaultConnection` connection string in `GymSystem.PL/appsettings.json`.
2. From the solution folder:
   ```bash
   dotnet run --project GymSystem.PL
   ```
   Migrations apply and seed data loads automatically on first run.
3. Open the shown `https://localhost:port`. Default route → `Home/Index`.

---

## 12. ✅ To-Do List — Status & Roadmap

Checklist of every module in the spec vs. what exists in the code today.

### ✔️ Done
- [x] **Project skeleton** — 3-layer architecture (PL / BLL / DAL)
- [x] **Database** — entities, EF configurations, constraints, initial migration
- [x] **Data seeding** — Plans & Trainers seeded from JSON on startup
- [x] **Member Management** — list, create, details, edit, delete (+ uniqueness & future-booking rules)
- [x] **Health Record** — view a member's health record
- [x] **Session Management** — list (with available slots) + create (with trainer/category validation)
- [x] **Plan Management** — list + details (routed through the BLL)
- [x] Cross-cutting: Result pattern, AutoMapper, Repository + UnitOfWork, DI

### 🚧 Not done yet (planned modules)

#### 1. Trainer Management — *CRUD* `TrainerController`
- [ ] `Index` – trainer listing page
- [ ] `Create` (GET form + POST handler) – with email/phone uniqueness & Egyptian phone validation
- [ ] `Details(id)` – trainer profile
- [ ] `Edit` (GET + POST)
- [ ] `Delete` + `DeleteConfirmed` – **block delete if the trainer has future sessions**
- [ ] Rule: must have exactly **one** speciality; HireDate auto-set

#### 2. Plan Management — *finish it* `PlansController`
- [ ] `Edit(id)` (GET form + POST handler)
- [ ] `Activate(id)` – toggle `IsActive` (soft delete / restore)
- [ ] Rule: **cannot edit/deactivate a plan that has active memberships**
- [ ] Rule: duration 1–365 days

#### 3. Session Management — *complete CRUD* `SessionsController`
- [ ] `Details(id)` – session details page
- [ ] `Edit` (GET + POST)
- [ ] `Delete` + `DeleteConfirmed` – **cannot delete a session with a future date**
- [ ] *(already done: Index + Create)*

#### 4. Membership Module — *new* `MemberPlanController` (assign plans to members)
- [ ] `Index` – memberships listing
- [ ] `Create` (GET + POST) – assign an **active** plan to an existing member
- [ ] `Cancel` – delete the member's membership on that plan
- [ ] Rules:
  - [ ] Member & Plan must exist
  - [ ] No more than **one active** membership at a time
  - [ ] Only **active** plans can be assigned
  - [ ] `EndDate = StartDate + Plan.DurationDays` (auto-calculated)
  - [ ] Status computed: `EndDate > Now` → "Active" else "Expired"
  - [ ] A membership can only be deleted while it is Active
- [ ] **Fix `Membership` model**: replace the placeholder `IEnumerable<Plan>/IEnumerable<Member>` with single `Plan`/`Member` references + migration

#### 5. Booking Module — *new* `MemberSessionController` (members book sessions)
- [ ] `Index` – sessions split into **Ongoing / Upcoming**
- [ ] `Create` (GET + POST) – book a member into a session
- [ ] `GetMembersForUpcomingSession(id)` – roster of an upcoming session
- [ ] `GetMembersForOngoingSessions(id)` – roster of an ongoing session (for attendance)
- [ ] `Cancel` – cancel a booking
- [ ] Rules:
  - [ ] Member must have an **active membership** to book
  - [ ] Session must have **available capacity** (not full)
  - [ ] Cannot book the **same session twice**
  - [ ] Only **future** sessions can be booked
  - [ ] Only **future** bookings can be cancelled
  - [ ] Attendance only markable for **ongoing** sessions; `IsAttended` defaults to `false`
  - [ ] Reject any action if the referenced booking/session doesn't exist
- [ ] **Fix `Booking` model**: replace the placeholder `IEnumerable<Session>/IEnumerable<Member>` with single `Session`/`Member` references + migration

#### 6. Dashboard — *new* `HomeController`
- [ ] Analytics & reports (member counts, active memberships, session occupancy, revenue, …)

#### 7. Identity Module — *new* (authentication & authorization)
- [ ] `ApplicationUser` (FirstName, LastName, UserName, Email, Phone) + `IdentityRole`
- [ ] `AccountController`: `Login` (GET + POST), `Logout`, `AccessDenied`
- [ ] Wire up `AddIdentity` + `UseAuthentication` in `Program.cs` (currently only `UseAuthorization` is present)
- [ ] Protect controllers/actions with `[Authorize]` + role checks

### 🛠️ Tech-debt / cleanups
- [ ] Reshape `Booking` & `Membership` navigations (see modules 4 & 5) — intentionally deferred until those modules are built
- [ ] Add validation summaries/partials to remaining views as they're created

> ℹ️ **Why the `Booking`/`Membership` placeholders exist:** both models currently expose `IEnumerable<>` navigation collections (e.g. `Booking.Sessions`) *alongside* a single FK (`Booking.SessionId`). EF uses the scalar FK, so the schema is correct today; the collections are kept intentionally as placeholders for the upcoming Booking & Membership modules.

---

### TL;DR mental model
> **Controller** takes the request → asks a **Service** → service applies **rules** and uses a **Repository** (via **UnitOfWork**) → repository talks to **EF Core / SQL** → data comes back as **Entities** → **AutoMapper** turns them into **ViewModels** → **View** renders HTML. Outcomes travel back as a **Result**. And remember: **`CreatedAt` is secretly StartDate/JoinDate/HireDate.**
