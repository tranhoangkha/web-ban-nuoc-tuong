using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BigSchool_1911067502.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigSchool_1911067502.Controllers
{
    public class CourseController : Controller
    {
        BigSchoolContext context = new BigSchoolContext();
        // GET: Course
        [Authorize]
        public ActionResult Create()
        {
            Course obj = new Course();
            obj.ListCategory = context.Categories.ToList();

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            ModelState.Remove("LecturerID");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Categories.ToList();
                return View("Create", objCourse);
            }

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerID = user.Id;

            context.Courses.Add(objCourse);
            context.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        public ActionResult Attending()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttend = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach(Attendance temp in listAttend)
            {
                Course objcourse = temp.Course;
                objcourse.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                                     .FindById(objcourse.LecturerID).Name;
                courses.Add(objcourse);
            }
            return View(courses);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var courses = context.Courses.Where(c => c.LecturerID == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            foreach (Course i in courses)
            {
                i.Name = currentUser.Name;
            }
            return View(courses);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Course course = context.Courses.FirstOrDefault(p => p.ID == id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        [HttpPost]
        public ActionResult Delete(Course course)
        {
            Course obj = context.Courses.FirstOrDefault(o => o.ID == course.ID);
            if (obj != null)
            {
                context.Courses.Remove(obj);
                context.SaveChanges();
            }
            return RedirectToAction("Mine", "Course");
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Course course = context.Courses.FirstOrDefault(p => p.ID == id);
            if (course == null)
            {
                return HttpNotFound();
            }
            course.ListCategory = context.Categories.ToList();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course course)
        {
            ModelState.Remove("LecturerID");
            if (ModelState.IsValid)
            {
                try
                {
                    var edit = context.Courses.FirstOrDefault(e => e.ID == course.ID);
                    ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    edit.LecturerID = user.Id;
                    edit.Place = course.Place;
                    edit.DateTime = course.DateTime;
                    edit.CategoryID = course.CategoryID;
                    context.Courses.AddOrUpdate(edit);
                    context.SaveChanges();
                    return RedirectToAction("Mine", "Course");
                }
                catch (Exception e)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                ModelState.AddModelError("", "Input Model Not Valid");
                return View(course);
            }
        }
    }
}