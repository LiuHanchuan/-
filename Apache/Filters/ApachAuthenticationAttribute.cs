using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Microsoft.AspNet.Identity;
using Apache.Models;
using Apache.ViewModels;
using System.Data.Entity;

namespace Apache.Filters
{
    public class ApachAuthenticationAttribute : FilterAttribute, IAuthenticationFilter
    {

        int notAuthentication = 1;  //设置未认证标志，用于区分返回不同的未认证页面
     
        public void OnAuthentication(AuthenticationContext context)
        {
            int caresult = 0;
            string currentUserName = context.HttpContext.User.Identity.GetUserName();
            if (context.HttpContext.User.Identity.IsAuthenticated && currentUserName!="admin"&& !(context.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) || context.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)))  //认证,并且用户名不等于admin及未设置Anonymous的用户执行下面代码 
            {
                notAuthentication = 0;  //设置已登录标志
                var uml = new UserMenuList();
                uml = (UserMenuList)context.HttpContext.Session["uml"];
                var currentController = context.RouteData.Values["controller"].ToString();
                var currentAction = context.RouteData.Values["action"].ToString();
                if (uml.Menus != null)
                {

                    //var tt = uml.Menus.Where(i => i.menuController.ToString() == currentController && i.menuAction == currentAction).ToList();
                    uml.Menus = uml.Menus.ToList();
                    foreach (var m in uml.Menus)
                    {
                        if (!(String.IsNullOrEmpty(m.menuAction) || String.IsNullOrEmpty(m.menuController)))
                        {
                            if (m.menuController.ToUpper() == currentController.ToUpper() && m.menuAction.ToUpper() == currentAction.ToUpper())
                            {
                                caresult = 1;
                                break;
                            }
                        }
                    }
                }

            }

            if ((context.HttpContext.User.Identity.IsAuthenticated && caresult == 1) || context.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) || context.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) || currentUserName=="admin")
            {
                // 已登录,并且查询数据库后具有操作权限或者设置了Anonymouse，以及当前用户是admin
            }
            else
            {
                context.Result = new HttpUnauthorizedResult(); // mark unauthorized
            }


            //if (context.HttpContext.User.Identity.IsAuthenticated &&
            //(context.HttpContext.User.IsInRole(superAdminRole)
            //|| context.HttpContext.User.IsInRole(adminRole))||(context.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)|| context.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)))
            //{
            //    // do nothing
            //}
            //else
            //{
            //    context.Result = new HttpUnauthorizedResult(); // mark unauthorized
            //}
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext context)
        {
            if (context.Result == null || context.Result is HttpUnauthorizedResult)
            {
                if (notAuthentication == 1)
                {
                    context.Result = new RedirectToRouteResult("Default",
                    new System.Web.Routing.RouteValueDictionary{
                   {"controller", "Account"},
                   {"action", "Login"},
                   {"returnUrl", context.HttpContext.Request.RawUrl}
                   });
                }
                else
                {
                    context.Result = new RedirectToRouteResult("Default",
                    new System.Web.Routing.RouteValueDictionary{
                   {"controller", "Account"},
                   {"action", "NotAuthentication"},
                   {"returnUrl", context.HttpContext.Request.RawUrl}
                   });
                }

            }
        }
    }
}