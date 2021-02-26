using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XkgWiki.Data;
using XkgWiki.Data.Repositories;
using XkgWiki.Domain;
using XkgWiki.Models.Pages;

namespace XkgWiki.Controllers
{
    public class PagesController : Controller
    {
        private readonly IPageRepository _pageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PagesController(IPageRepository pageRepository, IUnitOfWork unitOfWork)
        {
            _pageRepository = pageRepository;
            _unitOfWork = unitOfWork;
        }

        // GET: PagesController
        public async Task<ActionResult> List(CancellationToken cancellationToken)
        {
            var pageUrlsTitles = await _pageRepository.QueryAll().ToDictionaryAsync(p => p.Url, p => p.Title, cancellationToken);

            ViewBag.PageUrlsTitles = pageUrlsTitles;

            return View();
        }

        // GET: PagesController/Details/5
        [HttpGet("/Pages/Details/{url}")]
        public async Task<ActionResult> Details(string url)
        {
            var page = await _pageRepository.GetByUrlAsync(url);

            if (page == null)
                return NotFound();

            ViewBag.Page = page;

            return View();
        }

        // GET: PagesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] CreatePageRequestModel model, CancellationToken cancellationToken)
        {
            var page = new Page { Text = model.Text, Title = model.Title, Url = model.Url };

            _pageRepository.Save(page);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Ok();
        }

        // GET: PagesController/Edit/5
        [HttpGet("/Pages/Edit/{url}")]
        public async Task<ActionResult> Edit(string url)
        {
            var page = await _pageRepository.GetByUrlAsync(url);

            if (page == null)
                return BadRequest();

            ViewBag.Page = page;

            return View();
        }

        // POST: PagesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, [FromForm] UpdatePageRequestModel model, CancellationToken cancellationToken)
        {
            var page = await _pageRepository.GetByIdAsync(id, false);

            if (page == null)
                return BadRequest();

            page.Text = model.Text;
            page.Title = model.Title;
            page.Url = model.Url;

            _pageRepository.Save(page);
            await _unitOfWork.CommitAsync(cancellationToken);

            try
            {
                return RedirectToAction(nameof(Details), new { page.Url });
            }
            catch
            {
                return View();
            }
        }

        // POST: PagesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string url, CancellationToken cancellationToken)
        {
            var page = await _pageRepository.GetByUrlAsync(url);

            if (page == null)
                return BadRequest();

            _pageRepository.Delete(page);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Ok();
        }
    }
}