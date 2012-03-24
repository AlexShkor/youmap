using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YouMap.ActionFilters;
using YouMap.Admin;
using YouMap.Domain.Enums;
using YouMap.Framework;

namespace YouMap.Controllers
{
    [Admin]
    public class AdminController : BaseController
    {
        private readonly MongoRead _read;
        private readonly MongoWrite _write;
        private readonly Settings _settings;
        private readonly DeploymentHelper _deploymentHelper;
        //
        // GET: /Admin/

        public AdminController(MongoRead read, MongoWrite write, ICommandService commandService, Settings settings, DeploymentHelper deploymentHelper)
            : base(commandService)
        {
            _read = read;
            _write = write;
            _settings = settings;
            _deploymentHelper = deploymentHelper;
        }

        public ActionResult Index()
        {
            var model = new ReadModelGenerationModel();
            model.ReadConnectionString = _settings.MongoReadDatabaseConnectionString;
            model.WriteConnectionString = _settings.MongoWriteDatabaseConnectionString;
            model.CopyFromDatabase = _read.Database.Name;
            model.CopyToDatabase = _read.Database.Name;
            return View(model);
        }

        public ActionResult Regenerate(ReadModelGenerationModel model)
        {
            var sw = new Stopwatch();
            sw.Start();
            _deploymentHelper.RegenerateReadModel(model.ReadConnectionString, model.WriteConnectionString);
            sw.Stop();
            return Content("Done without errors. Elapsed time: " + sw.Elapsed.ToString());
        }

        public ActionResult GoToReadOnlyMode(string SetReadModeUrl)
        {
            _deploymentHelper.SwitchReadMode(SetReadModeUrl);

            return RedirectToAction("Index");
        }

        public ActionResult GoToWriteMode(string DisableReadModeUrl)
        {
            _deploymentHelper.SwitchReadMode(DisableReadModeUrl);

            return RedirectToAction("Index");
        }

    }

    public class ReadModelGenerationModel
    {
        public string ReadConnectionString { get; set; }

        public string WriteConnectionString { get; set; }

        public string CopyFromDatabase { get; set; }

        public string CopyToDatabase { get; set; }
    }
}
