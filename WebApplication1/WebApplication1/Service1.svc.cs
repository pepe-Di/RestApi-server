using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace WebApplication1
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service1
    {
        public UserEntities en = new UserEntities();
        [WebGet(UriTemplate = "/users")]
        public string GetAllUsers() 
        {
            List<string> list = new List<string>();
            foreach (User u in en.User) 
            {
                list.Add(u.Name.ToString());
            }
            return String.Join(", ",list);
        }

        [WebGet(UriTemplate = "/users?id={UserId}")]

        public string GetUserByID(string UserId)
        {
            if (!int.TryParse(UserId, out int id))
            {
                throw new HttpException("UserId must be an integer");
            }
            try
            {
                en.User.Find(id).Name.ToString();
            }
            catch (Exception e)
            {
                throw new HttpException("User doesn't exist.");
            }
            return en.User.Find(id).Name.ToString();
        }

        [WebGet(UriTemplate = "/users/{UserId}/photos")]
        
        public string GetUserPhotos(string UserId) 
        {
            List<string> links = new List<string>();
            if (!int.TryParse(UserId, out int id))
            {
                throw new HttpException("UserId must be an integer");
            }
            try
            {
                en.User.Find(id).Name.ToString();
            }
            catch (Exception e)
            {
                throw new HttpException("User doesn't exist.");
            }
            foreach (Photo p in en.Photo) 
            {
                if (p.userId.Equals(id)) 
                {
                    links.Add(p.Photo1.ToString());
                }
            }
            return String.Join(", ", links);
        }

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
     UriTemplate = "/users", ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.Wrapped)]

        public void AddUser(string log, string pass, string name) 
        {
            if (!log.Equals("") && !pass.Equals("") && !name.Equals(""))
            {
                foreach (User u in en.User)
                {
                    if (u.Login.ToString().Equals(log))
                    {
                        throw new HttpException("Login must be unique.");
                    }
                }
                en.User.Add(
                    new User()
                    {
                        Name = name,
                        Login = log,
                        Password = pass
                    });
                en.SaveChangesAsync();
            }
            else throw new HttpException("There are empty values.");
        }

        [WebInvoke(Method = "DELETE", RequestFormat = WebMessageFormat.Json,
    UriTemplate = "/users?id={UserId}", ResponseFormat = WebMessageFormat.Json,
    BodyStyle = WebMessageBodyStyle.Wrapped)]

        public void DeleteUser(string UserId)
        {
            if (!int.TryParse(UserId, out int id))
            {
                throw new HttpException("UserId must be an integer");
            }
            try
            {
                foreach (Photo p in en.Photo) 
                {
                    if (p.userId.Equals(id))
                            en.Photo.Remove(p);
                }
                en.User.Remove(en.User.Find(id));
                en.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new HttpException("User doesn't exist.");
            }
        }
        [WebInvoke(Method = "PUT", RequestFormat = WebMessageFormat.Json,
     UriTemplate = "/users?id={UserId}", ResponseFormat = WebMessageFormat.Json,
     BodyStyle = WebMessageBodyStyle.Wrapped)]
        public void UpdateUser(string UserId, string name)
        {
            if (name == "") 
            {
                throw new HttpException("Value is empty.");
            }
            if (!int.TryParse(UserId, out int id))
            {
                throw new HttpException("UserId must be an integer");
            }
            try
            {
                en.User.Find(id).Name=name;
                en.SaveChanges();
            }
            catch (Exception e)
            {
                throw new HttpException("User doesn't exist.");
            }
        }
    }
}
