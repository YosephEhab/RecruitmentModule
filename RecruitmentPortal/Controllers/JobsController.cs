using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentPortal.Data.Repositories;
using RecruitmentPortal.Models;

namespace RecruitmentPortal.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobRepository _jobRepository;

        public JobsController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<IActionResult> Index(int? page, string searchString, string currentFilter)
        {
            try
            {
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                ViewData["CurrentFilter"] = searchString;

                var jobs = _jobRepository.GetAll();
                if (!string.IsNullOrWhiteSpace(searchString)) jobs = jobs.Where(j => j.Name.Contains(searchString));

                return View(await PaginatedList<Job>.CreateAsync(jobs, page ?? 1, 3));
            }
            catch
            {
                return View(new PaginatedList<Job>(new List<Job>(), 0, 1, 3));
            }
        }

        [Authorize(Roles = Strings.Auth.Roles.Administrator)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = _jobRepository.GetWithSubmissions(id.Value);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [Authorize(Roles = Strings.Auth.Roles.Administrator)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Strings.Auth.Roles.Administrator)]
        public async Task<IActionResult> Create([Bind("Name,Description,Responsibilities,Skills,Category,ValidFrom,ValidUntil,MaxApplicants,ID")] Job job)
        {
            if (ModelState.IsValid)
            {
                if (job.ValidUntil <= job.ValidFrom)
                {
                    ModelState.AddModelError(nameof(job.ValidFrom), "Valid from cannot be greater than Valid until");
                    return View(job);
                }
                _jobRepository.Add(job);
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        [Authorize(Roles = Strings.Auth.Roles.Administrator)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = _jobRepository.Get(id.Value);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Strings.Auth.Roles.Administrator)]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Responsibilities,Skills,Category,ValidFrom,ValidUntil,MaxApplicants,ID")] Job job)
        {
            if (id != job.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (job.ValidUntil <= job.ValidFrom)
                {
                    ModelState.AddModelError(nameof(job.ValidFrom), "Valid from cannot be greater than Valid until");
                    return View(job);
                }
                try
                {
                    _jobRepository.Update(job);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        [Authorize(Roles = Strings.Auth.Roles.Administrator)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = _jobRepository.Get(id.Value);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Strings.Auth.Roles.Administrator)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = _jobRepository.Get(id);
            if (job != null)
            {
                _jobRepository.Delete(job);
            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Apply(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = _jobRepository.GetWithSubmissions(id.Value);
            if (job == null)
            {
                return NotFound();
            }
            if (!job.IsApplicable())
            {
                return RedirectToAction(actionName: "Error", controllerName: "Home", new { ErrorMessage = "This job has reached its maximum number of applicants" });
            }
            var jobSubmission = new JobSubmission() { Job = job, JobId = job.ID };

            return View(jobSubmission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("Name,Email,Mobile,JobId")] JobSubmission jobSubmission)
        {
            if (ModelState.IsValid)
            {
                if (!IsValidSubmission(jobSubmission, out string errorMessage))
                {
                    return RedirectToAction(actionName: "Error", controllerName: "Home", new { ErrorMessage = errorMessage });
                }
                _jobRepository.AddSubmission(jobSubmission);
                return RedirectToAction(nameof(Index));
            }
            return View(jobSubmission);
        }

        private bool IsValidSubmission(JobSubmission jobSubmission, out string errorMessage)
        {
            errorMessage = null;
            var job = _jobRepository.GetWithSubmissions(jobSubmission.JobId);

            if (job == null)
            {
                errorMessage = "Incorrect job ID";
                return false;
            }
            if (!job.IsApplicable())
            {
                errorMessage = "This job has reached its maximum number of applicants";
                return false;
            }
            var currentDate = DateTime.Now;
            if (currentDate < job.ValidFrom || currentDate > job.ValidUntil)
            {
                errorMessage = "This job is not available for submission";
                return false;
            }
            if (job.JobSubmissions.Any(s => s.Email.Equals(jobSubmission.Email, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "You have already applied to this job";
                return false;
            }

            return true;
        }

        private bool JobExists(int id)
        {
          return _jobRepository.GetAll().Any(e => e.ID == id);
        }
    }
}
