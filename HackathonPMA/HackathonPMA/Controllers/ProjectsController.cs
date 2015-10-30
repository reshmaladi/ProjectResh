using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HackathonPMA.Models;
using System.IO;
using Microsoft.Owin.Security;
using Microsoft.Reporting.WebForms;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Reflection;


namespace HackathonPMA.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private Entities db = new Entities();

        // GET: Projects
        public ActionResult Index(string sortBy, string currentFilter, string searchBy, int? page)
        {
            ViewBag.NameSort = string.IsNullOrEmpty(sortBy) ? "Name desc" : "";
            ViewBag.LocationSort = sortBy == "Location" ? "Location desc" : "Location";
            ViewBag.DescriptionSort = sortBy == "Description" ? "Description desc" : "Description";
            ViewBag.CategorySort = sortBy == "Category" ? "Category desc" : "Category";
            ViewBag.CitySort = sortBy == "City" ? "City desc" : "City";
            ViewBag.StartDateSort = sortBy == "StartDate" ? "StartDate desc" : "StartDate";
            ViewBag.EndDateSort = sortBy == "EndDate" ? "EndDate desc" : "EndDate";

            if (searchBy != null)
            {
                page = 1;
            }
            else
            {
                searchBy = currentFilter;
            }

            ViewBag.CurrentFilter = searchBy;

            List<Project> selProjects = new List<Project>();

            if (!User.IsInRole("Admin"))
            {
                string userId = User.Identity.GetUserId();
                var empProjects = db.EmployeeProjects;

                foreach (var ep in empProjects.ToList())
                {
                    if(ep.EmployeeId.Equals(userId))
                    {
                        Project p = db.Projects.Find(ep.ProjectId);
                        selProjects.Add(p);
                    }
                }
            } else {
                selProjects = db.Projects.ToList();
            }


            var projects = from s in selProjects
                           select s;

            if (!String.IsNullOrEmpty(searchBy))
            {
                projects = projects.Where(s => s.Name.Contains(searchBy)
                                       || s.Description.Contains(searchBy)
                                       || s.Location.Contains(searchBy)
                                       || s.Category.Contains(searchBy)
                                       || s.City.Contains(searchBy)
                                       || s.StartDate.ToString().Contains(searchBy)
                                       || s.EndDate.ToString().Contains(searchBy));
            }

            switch (sortBy)
            {
                case "Name desc":
                    projects = projects.OrderByDescending(s => s.Name);
                    break;
                case  "Name":
                    projects = projects.OrderBy(s => s.Name);
                    break;
                case "Location desc":
                    projects = projects.OrderByDescending(s => s.Location);
                    break;
                case "Location":
                    projects = projects.OrderBy(s => s.Location);
                    break;
                case "StartDate desc":
                    projects = projects.OrderByDescending(s => s.StartDate);
                    break;
                case "StartDate":
                    projects = projects.OrderBy(s => s.StartDate);
                    break;
                case "EndDate desc":
                    projects = projects.OrderByDescending(s => s.EndDate);
                    break;
                case "EndDate":
                    projects = projects.OrderBy(s => s.EndDate);
                    break;
                case "Category desc":
                    projects = projects.OrderByDescending(s => s.Category);
                    break;
                case "Category":
                    projects = projects.OrderBy(s => s.Category);
                    break;
                case "City desc":
                    projects = projects.OrderByDescending(s => s.City);
                    break;
                case "City":
                    projects = projects.OrderBy(s => s.City);
                    break;
                case "Description desc":
                    projects = projects.OrderByDescending(s => s.Description);
                    break;
                case "Description":
                    projects = projects.OrderBy(s => s.Description);
                    break;
                default:
                    projects = projects.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(projects.ToPagedList(pageNumber, pageSize));

           // return View(projects.ToList());
        }

        public ActionResult Report(string id)
        {
            LocalReport lr = new LocalReport();

            string path = Path.Combine(Server.MapPath("~/Reports"), "Report.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }

            List<Project> cm = new List<Project>();

            using (Entities es = new Entities())
            {
                cm = es.Projects.ToList();
            }

            ReportDataSource rd = new ReportDataSource("ProjectDataSet", cm);
            lr.DataSources.Add(rd);

            string reportType = id;
            string mimeType;
            string encoding;
            string fileNameExtension;

            string deviceInfo =

            "<DeviceInfo>" +
            "  <OutputFormat>" + id + "</OutputFormat>" +
            "  <PageWidth>8.5in</PageWidth>" +
            "  <PageHeight>11in</PageHeight>" +
            "  <MarginTop>0.5in</MarginTop>" +
            "  <MarginLeft>1in</MarginLeft>" +
            "  <MarginRight>1in</MarginRight>" +
            "  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            renderedBytes = lr.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            return File(renderedBytes, mimeType);
        }
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            double spentAmount = 0;
            var fundProjects = db.FundProjects;

            foreach (var fp in fundProjects.ToList())
            {
                if (fp.ProjectId == project.Id)
                {
                    spentAmount = spentAmount + Convert.ToDouble(fp.SpentAmount);
                }
            }
            
            ProjectDetailModel model = new ProjectDetailModel();            
            model.SpentAmount = spentAmount;
            model.project = project;
            var applicationDbContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(applicationDbContext));

            var eps = db.EmployeeProjects;
            foreach (var ep in eps.ToList())
            {
                if (ep.ProjectId == project.Id)
                {
                    var Db = new ApplicationDbContext();
                    var user = Db.Users.Find(ep.EmployeeId);
                    if(user != null)
                    {
                        Employee employee = new Employee();
                        employee.user = user;
                        var roles = userManager.GetRoles(user.Id); 
                        
                        if (roles != null)
                            employee.roleName = roles.First();
                        else
                            employee.roleName = "";
                        model.stakeholders.Add(employee);
                    }
                }
            }

            return View(model);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult CreateSubProject(int Id)
        {
            Project project = db.Projects.Find(Id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult CreateSubProject([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category")] Project project)
        {
            if (ModelState.IsValid)
            {
                Project mainproject = db.Projects.Find(project.Id);
                if (mainproject != null)
                {
                    mainproject.TotalSubProjects++;
                    if (mainproject.SubProjectIds != null)
                    {
                        if (mainproject.SubProjectIds.Length > 0) { 
                            mainproject.SubProjectIds = string.Concat(mainproject.SubProjectIds, ",");
                        }
                        mainproject.SubProjectIds = string.Concat(mainproject.SubProjectIds, project.Name);
                    }
                    else
                    {
                        mainproject.SubProjectIds = project.Name;
                    }
                    db.Entry(mainproject).State = EntityState.Modified;
                }

                Project subProject = new Project();
                subProject.StartDate = project.StartDate;
                subProject.EndDate = project.EndDate;
                subProject.City = project.City;
                subProject.Location = project.Location;

                subProject.Name = project.Name;
                subProject.Description = project.Description;
                subProject.Category = project.Category;

                subProject.CreatedOn = DateTime.Now;
                subProject.ModifiedOn = DateTime.Now;
                subProject.TotalSpentAmount = 0;
                subProject.ParentProjectId = project.Id;
                subProject.TotalSubProjects = 0;
                subProject.TotalAllocatedAmount = 0;
                subProject.IsParent = false;
                subProject.SubProjectIds = "";

                db.Projects.Add(subProject);
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }
        // GET: Projects/Create
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Create()
        {
             if (TempData["project"] != null)
            {
                TempData["project"] = TempData["project"];
                Project project = (Project)TempData["project"];
                return View(project);
            }
            return View();  
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin, Manager")]
        //public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category,State")] Project project, string btnAction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        project.IsParent = true;
        //        project.CreatedOn = DateTime.Now;
        //        project.ModifiedOn = DateTime.Now;
        //        project.TotalAllocatedAmount = 10000;
        //        project.TotalSpentAmount = 0;
        //        project.TotalSubProjects = 0;
        //        project.SubProjectIds = "";

        //        if (btnAction == "Next")
        //        {
        //            //TempData["pid"] = id;//
        //            TempData["project"] = project;
        //            return RedirectToAction("shMapping", "Account");
        //        }
        //        db.Projects.Add(project);
        //        db.SaveChanges();
        //         //ToAdd: start
        //        int id = project.Id;
                
        //        return RedirectToAction("Index");
        //    }

        //    return View(project);
        //}

        [MultiButton(MatchFormKey = "action", MatchFormValue = "Finish")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]        
        public ActionResult CreateFinish([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category,State")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.IsParent = true;
                project.CreatedOn = DateTime.Now;
                project.ModifiedOn = DateTime.Now;
                project.TotalAllocatedAmount = 10000;
                project.TotalSpentAmount = 0;
                project.TotalSubProjects = 0;
                project.SubProjectIds = "";               
                db.Projects.Add(project);
                db.SaveChanges();
                int id = project.Id;
                return RedirectToAction("Index");
            }
            return View(project);
        }


        [MultiButton(MatchFormKey = "action", MatchFormValue = "Next")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult CreateNext([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category,State")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.IsParent = true;
                project.CreatedOn = DateTime.Now;
                project.ModifiedOn = DateTime.Now;
                project.TotalAllocatedAmount = 10000;
                project.TotalSpentAmount = 0;
                project.TotalSubProjects = 0;
                project.SubProjectIds = "";

                //TempData["pid"] = id;//
                TempData["project"] = project;
                return RedirectToAction("shMapping", "Account");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,City,Location,Category")] Project project, string btnAction)
        {
            if (ModelState.IsValid)
            {
                Project p = db.Projects.Find(project.Id);
                p.Name = project.Name;
                p.Description = project.Description;
                p.StartDate = project.StartDate;
                p.EndDate = project.EndDate;
                p.City = project.City;
                p.Location = project.Location;
                p.Category = project.Category;
                p.ModifiedOn = DateTime.Now;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                //ToAdd:start
                int id = project.Id;
                if (btnAction == "Next")
                {
                    TempData["pid"] = id;
                    return RedirectToAction("shMapping", "Account");
                }
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            if (project != null)
            {
                int parentProjId = Convert.ToInt32(project.ParentProjectId);
                if (parentProjId > 0)
                {
                    Project mainProject = db.Projects.Find(project.ParentProjectId);
                    if (mainProject != null)
                    {
                        mainProject.TotalSubProjects--;
                        string subProjNames = mainProject.SubProjectIds;
                        if((subProjNames != null) && (subProjNames.Length > 0))
                        {
                            List<string> subProjs = subProjNames.Split(',').ToList<string>();
                            if(subProjs.Contains(project.Name))
                            {
                                subProjs.Remove(project.Name);
                                mainProject.SubProjectIds = string.Join(",", subProjs);
                            }
                        }
                        db.Entry(mainProject).State = EntityState.Modified;
                    }
                }

                db.Projects.Remove(project);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult doesProjectNameExist(string Name, string oldName)
        {
            if (Name.Contains(','))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            if (oldName.Equals("create") || (Name.Trim().ToLower() != oldName.Trim().ToLower()))
            {
                return Json(!db.Projects.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultiButtonAttribute : ActionNameSelectorAttribute
    {
        public string MatchFormKey { get; set; }
        public string MatchFormValue { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request[MatchFormKey] != null &&
                controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue;
        }
    }

    public class HttpParamActionAttribute : ActionNameSelectorAttribute
    {
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            if (actionName.Equals(methodInfo.Name, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (!actionName.Equals("Action", StringComparison.InvariantCultureIgnoreCase))
                return false;

            var request = controllerContext.RequestContext.HttpContext.Request;
            return request[methodInfo.Name] != null;
        }
    }
}
