using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DemoProject.Models;
using LoginPage.Models;
using RestSharp;
using Newtonsoft.Json;
using DemoProject.Data;
using MongoDB.Driver;
using System.Security.Cryptography.Xml;
using System.Linq;

namespace DemoProject.Controllers;

public class UserController : Controller {

    private readonly IMongoCollection<User> _users;
    public UserController(IConfiguration configuration){
        var _configuration = configuration;
        var _mongoClient = new MongoClient(_configuration["DatabaseSetting:ConnectionString"]);
        var _mongoDatabase = _mongoClient.GetDatabase(_configuration["DatabaseSetting:DatabaseName"]);
        _users = _mongoDatabase.GetCollection<User>(_configuration["DatabaseSetting:UserCollectionName"]);
    }

    public IActionResult Index() {
        return View();
    }

    public IActionResult Login(){
        return View();
    }

    [HttpPost]
    public IActionResult Login(User request){
        // User loginUser = _users.FirstOrDefault(u => u.Email == request.Email);
        var filter = Builders<User>.Filter.Eq(u => u.Email, request.Email);
        var loginUser = _users.Find(filter).FirstOrDefault();
        if(loginUser != null && loginUser.Password == request.Password){
            //Console.WriteLine("login successful: "+loginUser.BoredFactData.Activity.ToString());
            //return RedirectToAction("Home", loginUser);
            //return Content(loginUser.BoredFactData.Activity);
            return RedirectToAction("Home", new { data = JsonConvert.SerializeObject(loginUser) });
        }
        else{
            return Content("Login Failed!!!");
        }
    }

    public IActionResult Register(){
        return View();
    }

    [HttpPost]
    public IActionResult Register(User request){
        try
            {
                User user = new(){
                    Name = request.Name,
                    Email = request.Email,
                    Password = request.Password
                };
                User IsUserInDb = _users.Find(u => u.Email == request.Email).FirstOrDefault();
                if(IsUserInDb != null && user.Email.Equals(IsUserInDb.Email)){
                    //return RedirectToAction("Login");
                    return Content("Email Already Exists!");
                }
                _users.InsertOne(user);
                return RedirectToAction("Login");

            }

            catch (Exception e)

            {

                return View("Unexpected Error : " + e.Message);

            }
    }

    public IActionResult Home(string data){
        try{
                User? loginUser = JsonConvert.DeserializeObject<User>(data);
                if(loginUser?.BoredFactData != null)
                    return View("Home", loginUser.BoredFactData);

                var client = new RestClient("https://www.boredapi.com/api/activity/");
                var request = new RestRequest();
                var response = client.Get(request);
                BoredFact? bored = JsonConvert.DeserializeObject<BoredFact>(response.Content);
                var filter = Builders<User>.Filter.Eq(u => u.Email, loginUser?.Email);
                var update = Builders<User>.Update.Set(u => u.BoredFactData, bored);
                var updateResult = _users.UpdateOne(filter, update);
                loginUser.BoredFactData = bored;
                if (loginUser != null && loginUser.BoredFactData != null)
                {   
                    return View("Home", loginUser.BoredFactData);
                }
                else
                {
                    return Content("Failed to store in database");
                }
            }
            catch (Exception e){

                return Json("Error" + e.Message);

            }
    }

    public IActionResult ForgotPassword(){
        return View();
    }
    //debug ForgotPassword to see what filter is storing
    [HttpPost]
    public IActionResult ForgotPassword(ForgotPasswordModel request){
        User userFromDb = _users.Find(u => u.Email == request.Email).FirstOrDefault();
        if(userFromDb != null && userFromDb.Email.Equals(request.Email)){
            if(request.Password.Equals(request.ConfirmPassword)){
                userFromDb.Password = request.Password;
                var filter = Builders<User>.Filter.Eq(u => u.Email, request.Email);
                var update = Builders<User>.Update.Set(u => u.Password, request.Password);
                _users.UpdateOne(filter, update);
                return RedirectToAction("Login");
            }
        }
        return RedirectToAction("ForgotPassword");
    }
}