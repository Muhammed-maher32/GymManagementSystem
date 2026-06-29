using GymSystem.BLL.Services.Contracts;
using GymSystem.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymSystemG04.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);

            return View(members.Value);
        }

        #region Create
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);
            var result = await _memberService.CreateMemberAsync(model, ct);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return View(nameof(Create), model);
            }
            TempData["SuccessMessage"] = "Member Created Successfully";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Details

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (!member.Success)
            {
                TempData["ErrorMessage"] = member.Error;
                return RedirectToAction(nameof(Index));
            }

            return View(member.Value);
        }


        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (!healthRecord.Success)
            {
                TempData["ErrorMessage"] = healthRecord.Error;
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord.Value);
        }

        #endregion

        #region Update

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (!member.Success)
            {
                TempData["ErrorMessage"] = member.Error;
                return RedirectToAction(nameof(Index));
            }

            return View(member.Value);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberAsync(id, model, ct);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Member Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (!member.Success)
            {
                TempData["ErrorMessage"] = member.Error;
                return RedirectToAction(nameof(Index));
            }

            return View(member.Value);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
            }

            return RedirectToAction(nameof(Index));
        }



        #endregion


    }
}
