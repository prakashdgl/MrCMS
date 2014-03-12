﻿using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models.Search;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SearchController : MrCMSAdminController
    {
        private readonly ISiteMapService _siteMapService;
        private readonly IAdminWebpageSearchService _adminWebpageSearchService;

        public SearchController(ISiteMapService siteMapService, IAdminWebpageSearchService adminWebpageSearchService)
        {
            _siteMapService = siteMapService;
            _adminWebpageSearchService = adminWebpageSearchService;
        }

        [HttpGet]
        public ActionResult Index(AdminWebpageSearchQuery model)
        {
            ViewData["parents"] = _adminWebpageSearchService.GetParentsList();
            ViewData["doc-types"] = _adminWebpageSearchService.GetDocumentTypes(model.Type);
            ViewData["results"] = _adminWebpageSearchService.Search(model);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSearchResults(AdminWebpageSearchQuery model)
        {
            if (string.IsNullOrWhiteSpace(model.Term))
                return Json(new object());
            return Json(_adminWebpageSearchService.QuickSearch(model), JsonRequestBehavior.AllowGet);
        }

        
        public PartialViewResult GetBreadCrumb(int? parentId)
        {
            if (parentId.HasValue)
                return PartialView(_adminWebpageSearchService.GetBreadCrumb(parentId));
            return PartialView(null);
        }
    }
}