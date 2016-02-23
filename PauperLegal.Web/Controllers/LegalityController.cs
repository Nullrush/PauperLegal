using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using PauperLegal.Web.Providers;

namespace PauperLegal.Web.Controllers
{
    public class LegalityController : Controller
    {
        private readonly ILegalityRepository _legalityRepository;

        public LegalityController(ILegalityRepository repository)
        {
            _legalityRepository = repository;
        }

        public ActionResult Index()
        {
            return new JsonResult { Data = new { isLegal = _legalityRepository.IsCardLegal("junk") }, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }
    }
}
