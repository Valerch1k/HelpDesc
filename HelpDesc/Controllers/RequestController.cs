using HelpDesc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace HelpDesc.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        HelpdeskContext db = new HelpdeskContext();
        [HttpGet]
        public ActionResult Index()
        {
            // получаем текущего пользователя
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).FirstOrDefault();
            IOrderedQueryable<Request> requests;
            requests = db.Requests.Where(r => r.UserId == user.Id) //получаем заявки для текущего пользователя
                                  .Include(r => r.Category)  // добавляем категории
                                  .Include(r => r.Lifecycle)  // добавляем жизненный цикл заявок
                                  .Include(r => r.User)         // добавляем данные о пользователях
                                  .OrderByDescending(r => r.Lifecycle.Opened); // упорядочиваем по дате по убыванию  
            if (HttpContext.User.IsInRole("Администратор") || HttpContext.User.IsInRole("Исполнитель") || HttpContext.User.IsInRole("Модератор"))
            {
                AdminRequest(user);
            }
            else
            {   
                UserRequest(user);
            }

            return View(requests.ToList());
         
        }

        [HttpGet]
        public ActionResult Create()
        {
            // получаем текущего пользователя
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                // получаем набор кабинетов для департамента, в котором работает пользователь
                var cabs = from cab in db.Activs
                           where cab.DepartmentId == user.DepartmentId
                           select cab;
                ViewBag.Cabs = new SelectList(db.Activs, "Id", "CabNumber");

                ViewBag.Categories = new SelectList(db.Categories, "Id", "Name");
                return View();
            }
            return RedirectToAction("LogOff", "Account");
        }

        // Создание новой заявки
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Request request, HttpPostedFileBase error)
        {
            ViewBag.Cabs = new SelectList(db.Activs, "Id", "CabNumber");
            ViewBag.Categories = new SelectList(db.Categories, "Id", "Name");
            // получаем текущего пользователя
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("LogOff", "Account");
            }
            if (ModelState.IsValid)
            {
                // указываем статус Открыта у заявки
                request.Status = (int)RequestStatus.Open;
                //получаем время открытия
                DateTime current = DateTime.Now;

                //Создаем запись о жизненном цикле заявки
                Lifecycle newLifecycle = new Lifecycle() { Opened = current };
                request.Lifecycle = newLifecycle;

                //Добавляем жизненный цикл заявки
                db.Lifecycles.Add(newLifecycle);

                // указываем пользователя заявки
                request.UserId = user.Id;

                // если получен файл
                if (error != null)
                {
                    // Получаем расширение
                    string ext = error.FileName.Substring(error.FileName.LastIndexOf('.'));
                    // сохраняем файл по определенному пути на сервере
                    string path = current.ToString("dd/MM/yyyy H:mm:ss").Replace(":", "_").Replace("/", ".") + ext;
                    error.SaveAs(Server.MapPath("~/Files/" + path));
                    request.File = path;
                }
                //Добавляем заявку
                db.Requests.Add(request);
                db.SaveChanges();

                // отправляем mail уведомление про созданя заявки ******************************
                //получаем номер кабинета и название отдела 
                var DepartActiv = db.Activs.Where(m => m.Id == request.ActivId)
                                           .Join(db.Departments, a =>a.DepartmentId , d => d.Id ,
                                           (a, d) => new {a.CabNumber,d.Name }).FirstOrDefault();
                // название категории проблемы 
                Category categori = db.Categories.Where(m => m.Id == request.CategoryId).FirstOrDefault();

                var mailController = new EmailProvider();
                var mailtemplate = new EmailTemplate() { UserName = user.Name, UserLastname = user.LastName,UserEmail = user.Email, UserPosition = user.Position, UserDepartment = DepartActiv.Name , UserPhoneNumber = user.PhoneNumber ,
                                                        RequestName = request.Name, RequestActiv = DepartActiv.CabNumber, RequestCategory = categori.Name, RequestComment = request.Comment , RequestDescription = request.Description, RequestPriority = request.Priority};
                var email = mailController.Subscription(mailtemplate, "valerchik396@gmail.com");
                email.Deliver();
                // конец  отправляем mail уведомление про созданя заявки ***********************
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // Просмотр подробных сведений о заявке
        [HttpGet]
        public ActionResult Details(int id)
        {
            Request request = db.Requests.Find(id);

            if (request != null)
            {
                //получаем кабинет
                var activ = db.Activs.Where(m => m.Id == request.ActivId);
                // так как кабинет у нас может быть не указан, и набор может возвращать 0 значений
                if (activ.Count() > 0)
                {
                    request.Activ = activ.First();
                }
                //получаем категорию
                request.Category = db.Categories.Where(m => m.Id == request.CategoryId).First();
                return PartialView("_Details", request);
            }

            return View("Index");
        }

        //данные об исполнителе
        public ActionResult Executor(int id)
        {
            User executor = db.Users.Where(m => m.Id == id).First();

            if (executor != null)
            {
                return PartialView("_Executor", executor);
            }
            return View("Index");
        }

        // информация про жизненный цикл заявки 
        public ActionResult Lifecycle(int id)
        {
            Lifecycle lifecycle = db.Lifecycles.Where(m => m.Id == id).First();

            if (lifecycle != null)
            {
                return PartialView("_Lifecycle", lifecycle);
            }
            return View("Index");
        }

        // Удаление заявки
        public ActionResult Delete(int id)
        {
            Request request = db.Requests.Find(id);
            // получаем текущего пользователя
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            if (request != null && request.UserId == user.Id)
            {
                Lifecycle lifecycle = db.Lifecycles.Find(request.LifecycleId);
                db.Lifecycles.Remove(lifecycle);
                db.SaveChanges();
            }
            
            return RedirectToAction("index");
        }

        // загружаем файл
        public ActionResult Download(int id)
        {
            Request r = db.Requests.Find(id);
            if (r != null)
            {
                string filename = Server.MapPath("~/Files/" + r.File);
                string contentType = "image/jpeg";

                string ext = filename.Substring(filename.LastIndexOf('.'));
                switch (ext)
                {
                    case "txt":
                        contentType = "text/plain";
                        break;
                    case "png":
                        contentType = "image/png";
                        break;
                    case "tiff":
                        contentType = "image/tiff";
                        break;
                }
                return File(filename, contentType, filename);
            }

            return Content("Файл не найден");
        }

        // просмотр заявок админом , видит всех и вся 
        [Authorize(Roles = "Администратор")]
        public ActionResult RequestList()
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).FirstOrDefault();
            var requests = db.Requests.Include(r => r.Category)
                                    .Include(r => r.Lifecycle)
                                    .Include(r => r.User)
                                    .OrderByDescending(r => r.Lifecycle.Opened);

            if (HttpContext.User.IsInRole("Администратор") || HttpContext.User.IsInRole("Исполнитель") || HttpContext.User.IsInRole("Модератор"))
            {
                AdminRequest(user);
            }
            else
            {
                UserRequest(user);
            }
            return View(requests.ToList());
        }

        //Модерирование заявок модератором
        [HttpGet]
        [Authorize(Roles = "Администратор, Модератор")]
        public ActionResult Distribute()
        {
            var requests = db.Requests.Include(r => r.User)
                                    .Include(r => r.Lifecycle)
                                    .Include(r => r.Executor)
                                    .Where(r => r.ExecutorId == null)
                                    .Where(r => r.Status != (int)RequestStatus.Closed);
            List<User> executors = db.Users.Include(e => e.Role)
                                        .Where(e => e.Role.Name == "Исполнитель" || e.Role.Name == "Администратор").ToList<User>();

            ViewBag.Executors = new SelectList(executors, "Id", "Name");
            return View(requests);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Модератор")]
        public ActionResult Distribute(int? requestId, int? executorId)
        {
            if (requestId == null && executorId == null)
            {
                return RedirectToAction("Distribute");
            }
            Request req = db.Requests.Find(requestId);
            User ex = db.Users.Find(executorId);
            if (req == null && ex == null)
            {
                return RedirectToAction("Distribute");
            }
            req.ExecutorId = executorId;

            req.Status = (int)RequestStatus.Distributed;
            Lifecycle lifecycle = db.Lifecycles.Find(req.LifecycleId);
            lifecycle.Distributed = DateTime.Now;
            db.Entry(lifecycle).State = EntityState.Modified;

            db.Entry(req).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Distribute");
        }

        //Заявки для изменения статуса исполнителем
        [HttpGet]
        [Authorize(Roles = "Администратор, Исполнитель")]
        public ActionResult ChangeStatus()
        {
            // получаем текущего пользователя
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            if (user != null)
            {
                var requests = db.Requests.Include(r => r.User)
                                    .Include(r => r.Lifecycle)
                                    .Include(r => r.Executor)
                                    .Where(r => r.ExecutorId == user.Id)
                                    .Where(r => r.Status != (int)RequestStatus.Closed);
                return View(requests);
            }
            return RedirectToAction("LogOff", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "Администратор,Исполнитель")]
        public ActionResult ChangeStatus(int requestId, int status)
        {
            User user = db.Users.Where(m => m.Login == HttpContext.User.Identity.Name).First();
            if (user == null)
            {
                return RedirectToAction("LogOff", "Account");
            }

            Request req = db.Requests.Find(requestId);
            if (req != null)
            {
                req.Status = status;
                Lifecycle lifecycle = db.Lifecycles.Find(req.LifecycleId);
                if (status == (int)RequestStatus.Proccesing)
                {
                    lifecycle.Proccesing = DateTime.Now;
                }
                else if (status == (int)RequestStatus.Checking)
                {
                    lifecycle.Checking = DateTime.Now;
                }
                else if (status == (int)RequestStatus.Closed)
                {
                    lifecycle.Closed = DateTime.Now;
                }
                db.Entry(lifecycle).State = EntityState.Modified;
                db.Entry(req).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ChangeStatus");
            }

        [HttpGet]
        public ActionResult MoreRequest( int id)
        {
            Request request = db.Requests.Find(id);
            //получаем кабинет
            var activ = db.Activs.Where(m => m.Id == request.ActivId);
            // так как кабинет у нас может быть не указан, и набор может возвращать 0 значений
            if (activ.Count() > 0)
            {
                request.Activ = activ.First();
            }
            //получаем категорию
            request.Category = db.Categories.Where(m => m.Id == request.CategoryId).First();
            // жизенный цикл
            request.Lifecycle = db.Lifecycles.Where(m => m.Id == request.LifecycleId).First();
            // кому пренадлежит 
            request.User = db.Users.Where(m=>m.Id == request.UserId).First();
            //  кто исполнетель 
            var Executor = db.Users.Where(m => m.Id == request.ExecutorId);
            //  проверяем назначен ли исполнитель 
            if (Executor.Count() > 0)
            {
                request.Executor = Executor.First();
            }

            return View(request);
        }

        private void UserRequest(User user)
        {
            var DistributedRequestsCount = db.Requests.Include(r => r.User)
                                  .Where(r => r.UserId == user.Id)
                                  .Where(r => r.Status == (int)RequestStatus.Distributed)
                                  .Count();
            ViewBag.DistributedRequestsCount = DistributedRequestsCount;
            var ProccesingRequestsCount = db.Requests.Include(r => r.User)
                               .Where(r => r.UserId == user.Id)
                               .Where(r => r.Status == (int)RequestStatus.Proccesing)
                               .Count();
            ViewBag.ProccesingRequestsCount = ProccesingRequestsCount;
            var CheckingRequestsCount = db.Requests.Include(r => r.User)
                               .Where(r => r.UserId == user.Id)
                               .Where(r => r.Status == (int)RequestStatus.Checking)
                               .Count();
            ViewBag.CheckingRequestsCount = CheckingRequestsCount;
            var ClosedRequestsCount = db.Requests.Include(r => r.User)
                               .Where(r => r.UserId == user.Id)
                               .Where(r => r.Status == (int)RequestStatus.Closed)
                               .Count();
            ViewBag.ClosedRequestsCount = ClosedRequestsCount;
        }

        private void AdminRequest(User user)
        {
            // количество назначенных заявок 
            var MyRequestsCount = db.Requests.Include(r => r.User)
                                .Where(r => r.ExecutorId == user.Id)
                                .Where(r => r.Status != (int)RequestStatus.Closed && r.Status != (int)RequestStatus.Open)
                                .Count();
            ViewBag.MyRequestsCount = MyRequestsCount;
            // количество не  назначенных заявок 
            var RequestsCountOpen = db.Requests.Include(r => r.User)
                                .Where(r => r.Status == (int)RequestStatus.Open)
                                .Count();
            ViewBag.RequestsCountOpen = RequestsCountOpen;
            // количество важных заявок 
            var RequestsCountCritical = db.Requests.Include(r => r.User)
                                //  .Where(r => r.ExecutorId == user.Id)
                                .Where(r => r.Priority == (int)RequestPriority.Critical)
                                .Count();
            ViewBag.RequestsCountCritical = RequestsCountCritical;
            // количество новых заявок за день 
            var RequestsCountAllDay = db.Requests.Include(r => r.User)
                                .Include(r => r.Lifecycle)  // добавляем жизненный цикл заявок
                                .Where(r => r.Lifecycle.Opened > DateTime.Today)
                                .Count();
            ViewBag.RequestsCountAllDay = RequestsCountAllDay;
        }

    }
}
