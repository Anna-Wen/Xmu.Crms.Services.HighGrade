using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Service;
using System.Numerics;
using System.Text;
using System.Security;

using System.Security.Cryptography;


namespace Xmu.Crms.Services.HighGrade
{
    /// <summary>
    /// @author Group HighGrade
    /// @version 2.00
    /// </summary>
    public class LoginService : ILoginService
    {

        private readonly CrmsContext _db;

        public LoginService(CrmsContext db)
        {
            _db = db;
        }

        public string GetMd5(string strPwd)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] byteHash = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strPwd));
                string strRes = BitConverter.ToString(byteHash).Replace("-", "");
                return strRes.ToUpper();
            }
        }

        /// <summary>
        /// 微信登录.
        /// @author Group HighGrade
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="code">微信小程序/OAuth2授权的Code</param>
        /// <param name="state">微信OAuth2授权的state。对于小程序，值恒为 MiniProgram</param>
        /// <param name="successUrl">微信OAuth2授权后跳转到的网址</param>
        /// <returns>user 该用户信息</returns>
        public UserInfo SignInWeChat(long userId, string code, string state, string successUrl)
        {
            var us = new UserInfo();
            return us;
        }

        /// <summary>
        /// 手机号登录.
        /// @author Group HighGrade
        /// User中只有phone和password，用于判断用户名密码是否正确
        /// </summary>
        /// <param name="user">用户信息(手机号Phone和密码Password)</param>
        /// <returns>user 该用户信息</returns>
        public UserInfo SignInPhone(UserInfo user)
        {
            //	MD5 md5I = new MD5CryptoServiceProvider();

            var us = _db.UserInfo.SingleOrDefault(u => u.Phone == user.Phone);
            if (us == null)
            {
                throw new UserNotFoundException();
            }
            /*  byte[] byteArray1= System.Text.Encoding.Default.GetBytes(user.Password);
              byte[] byteArray2 = System.Text.Encoding.Default.GetBytes(us.Password); 

             if (md5I.ComputeHash(byteArray1) !=byteArray2)
              {
                  throw new PasswordErrorException();
              }       */
            if (GetMd5(user.Password) != us.Password)
            {
                throw new PasswordErrorException();
            }



            return us;
        }

        /// <summary>
        /// 手机号注册.
        /// @author Group HighGrade
        /// 手机号注册 User中只有phone和password，userId是注册后才有并且在数据库自增
        /// </summary>
        /// <param name="user">用户信息(手机号Phone和密码Password)</param>
        /// <returns>user 该用户信息</returns>
        public UserInfo SignUpPhone(UserInfo user)
        {
            // MD5 md5 = new MD5CryptoServiceProvider();
            //byte[] byteArray1= System.Text.Encoding.Default.GetBytes(user.Password);


            var u = new UserInfo
            {
                Phone = user.Phone,
                Password = GetMd5(user.Password),
            };


            //Password= md5.ComputeHash(user.Password)


            _db.UserInfo.Add(u);
            _db.SaveChanges();
            return u;
        }

        /// <summary>
        /// 用户解绑.教师解绑账号(j2ee)
        /// @author Group HighGrade
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>true 解绑成功 false 解绑失败</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ICourseService.ListCourseByUserId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ICourseService.DeleteCourseByCourseId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">未找到对应用户</exception>
        public void DeleteTeacherAccount(long userId)
        {
            var teacher = _db.UserInfo.SingleOrDefault(u => u.Id == userId);

            if (teacher == null)
                throw new UserNotFoundException();

            teacher.Phone = null;
            _db.SaveChanges();
        }

        /// <summary>
        /// 用户解绑.学生解绑账号(j2ee)
        /// @author Group HighGrade
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>true 解绑成功 false 解绑失败</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IClassService.DeleteCourseSelectionById(System.Int64,System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">未找到对应用户</exception>
        public void DeleteStudentAccount(long userId)
        {
            var student = _db.UserInfo.SingleOrDefault(u => u.Id == userId);

            if (student == null)
                throw new UserNotFoundException();

            student.Phone = null;
            _db.SaveChanges();
        }

    }
}