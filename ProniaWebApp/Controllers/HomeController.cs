using Microsoft.AspNetCore.Mvc;
using ProniaWebApp.Models;
using ProniaWebApp.ViewModels;

namespace ProniaWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Slide> slides = new List<Slide>
            {
                new Slide
                {
                    Id = 1,
                    Title = "Home1",
                    SubTitle = "Home1",
                    Description = "PB 201",
                    Order = 1,
                    ImageURL = "1-2-524x617.png"
                },
                new Slide
                {
                    Id = 2,
                    Title = "Home2",
                    SubTitle = "Home2",
                    Description = "Welcome",
                    Order = 3,
                    ImageURL = "1-1-524x617.png"
                },
                new Slide
                {
                    Id = 3,
                    Title = "Home",
                    SubTitle = "Home",
                    Description = "Hello, welcome Home",
                    Order = 2,
                    ImageURL = "1-2-524x617.png"
                },

            };

            HomeVM homeVM = new HomeVM 
            { 
                Slides = slides.OrderBy(x => x.Order).ToList(),
            }; 
            return View(homeVM);
        }
    }
}
