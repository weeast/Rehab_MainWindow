using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Data.Linq;

namespace Rehab
{
    using Rehab_MainWindow;
    public class basicRehab
    {        
        public static MySqlConnection con;
        
        public static string conSQL = "server = 111.67.201.113;user=root;password=85589890;port=3306;database=rehab;";
        //public static string conSQL = "server = localhost;user=root;password=root;port=3306;database=rehab;";
        public static void myopen()
        {
            if(con==null)
            {
                //string sql = "server=" + server + ";user=" + user + ";password=" + password + ";port=" + port + ";database=" + database;
                con = new MySqlConnection(conSQL);
                con.Open();
            }
            else if(con.State != ConnectionState.Connecting && con.State!=ConnectionState.Open)
            {
                con.Open();
            }
        }
        public static void myclose()
        {
            if (con.State != ConnectionState.Closed)
            {
                con.Close();
            }
        }
        public static bool isEmail(string test)
        {
            return Regex.IsMatch(test, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase);
        }
       
        /*
         * public static bool HasEmail(string source)
        {
            return Regex.IsMatch(source, @"[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})", RegexOptions.IgnoreCase);
        }
         */
    }
    public class patient : basicRehab
    {
        public int pID { get; set; }                //病例号
        public int puid { get; set; }               //用户号
        public int pSex { get; set; }               //性别
        public int pAge { get; set; }               //年龄
        public string pCareer { get; set; }         //职业
        public string pPhone { get; set; }          //电话
        public string pEmail { get; set; }          //邮箱
        public string pWorkUnit { get; set; }       //工作单位
        public string pConAddress { get; set; }     //联系地址
        public string pOpHistory { get; set; }      //手术史
        public string pAllergy { get; set; }        //过敏史
        public string pInjuredPart { get; set; }    //受伤部位
        public string pHospital { get; set; }       //医院
        public string pDiagnosis { get; set; }      //医生诊断
        public string pInspectlist { get; set; }    //检查单
        public string pRname { get; set; }          //真实姓名
    
//        create table t_student
//(
//  id varchar(50) primary key,
//  name varchar(50) ,
//  address varchar(50)


        //直接patient.QueryAll(puid)调用此函数 puid为user的uid。
        //返回list表
        public static List<patient> QueryAll(int puid)//string[] column, MySqlConnection dd)
        {

            List<patient> list = new List<patient>();
            string sql = "select * from patient where puid="+puid+";";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                patient pt1 = new patient();
                //if (row["id"] != null)
                //{
                    pt1.pID = Convert.ToInt32(row["pid"]);
                    pt1.puid = Convert.ToInt16(row["puid"]);
                    pt1.pSex = Convert.ToByte(row["psex"]);
                    pt1.pAge = Convert.ToInt16(row["page"]);
                    pt1.pCareer = Convert.ToString(row["pcareer"]);
                    pt1.pPhone = Convert.ToString(row["pphone"]);
                    pt1.pEmail = Convert.ToString(row["pemail"]);
                    pt1.pWorkUnit = Convert.ToString(row["pworkunit"]);
                    pt1.pConAddress = Convert.ToString(row["pconaddress"]);
                    pt1.pOpHistory = Convert.ToString(row["pophistory"]);
                    pt1.pAllergy = Convert.ToString(row["pallergy"]);
                    pt1.pInjuredPart = Convert.ToString(row["pinjuredpart"]);
                    pt1.pHospital = Convert.ToString(row["phospital"]);
                    pt1.pDiagnosis = Convert.ToString(row["pdiagnosis"]);
                    pt1.pInspectlist = Convert.ToString(row["pinspectlist"]);        
                    pt1.pRname = Convert.ToString(row["prname"]);

                    list.Add(pt1);
                //}
            }
            return list;
        }

        public static List<doctor> queryAllDoctors(int uid)
        {
            string sql = "select * from doctor;"; //AS d INNER JOIN patient_doctor AS pd INNER JOIN patient AS p ON d.did=pd.pddid AND p.pid=pd.pdpid AND p.puid="+uid+";";
            List<doctor> list = new List<doctor>();
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                doctor temp = new doctor();
                temp.did = Convert.ToInt16(row["did"]);
                temp.duid = Convert.ToInt16(row["duid"]);
                temp.dage = Convert.ToInt16(row["dage"]);
                temp.drname = Convert.ToString(row["drname"]);
                temp.dsex = Convert.ToInt16(row["dsex"]);
                temp.dIDcard = Convert.ToString(row["dIDcard"]);
                temp.dworkunit = Convert.ToString(row["dworkunit"]);
                temp.dworkpart = Convert.ToString(row["dworkpart"]);
                temp.dlevel = Convert.ToString(row["dlevel"]);
                temp.dphone = Convert.ToString(row["dphone"]);
                //temp.pAllergy = Convert.ToString(row["pallergy"]);
                temp.dresume = Convert.ToString(row["dresume"]);

                list.Add(temp);
            }
            return list;
        }

        public static List<doctor> queryDoctor(int uid)
        {
            List<doctor> list = new List<doctor>();
            string sql = "select d.* from doctor AS d INNER JOIN patient_doctor AS pd INNER JOIN patient AS p ON d.did=pd.pddid AND p.puid="+uid+";";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                doctor temp = new doctor();
                temp.did = Convert.ToInt16(row["did"]);
                temp.duid= Convert.ToInt16(row["duid"]);
                temp.dage = Convert.ToInt16(row["dage"]);
                temp.dsex = Convert.ToInt16(row["dsex"]);
                temp.drname = Convert.ToString(row["drname"]);
                temp.dIDcard = Convert.ToString(row["dIDcard"]);
                temp.dworkunit = Convert.ToString(row["dworkunit"]);
                temp.dworkpart = Convert.ToString(row["dworkpart"]);
                temp.dlevel = Convert.ToString(row["dlevel"]);
                temp.dphone = Convert.ToString(row["dphone"]);
                //temp.pAllergy = Convert.ToString(row["pallergy"]);
                temp.dresume = Convert.ToString(row["dresume"]);
                
                list.Add(temp);
            }
            return list;
        }
        //参数为string数组，依次为puid,psex,page,pcareer,pphone,pemail,pworkunit,
        //pconaddress,pophistory,pallergy,pinjuredpart,phospital,pdiagnosis,pinspectlist,pidcard,prname
        //返回bool值     测试接口...   有错速报
        public static bool insert(string[] input)
        {
            myopen();
            string sql = "insert into patient(pid,puid,psex,page,pcareer,pphone,pemail,pworkunit,pconaddress,pophistory,pallergy,pinjuredpart,phospital,pdiagnosis,pinspectlist,pidcard,prname) values(";
            if (input.Length > 12)
            {
                myclose();
                return false;
            }
            for (int i = 0; i < input.Length-1; i++)
            {
                if (i < 4)
                {
                    sql += input[i] + ",";
                }
                else
                {
                    sql += "'" + input[i] + "',";
                }
            }
            sql += "'" + input[input.Length - 1] + "');";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
                string sql1 = "update user set uall=1 where uid =" + input[0] + ";";
                MySqlCommand cmd1 = new MySqlCommand(sql1, con);
                try
                {
                    myopen();
                    cmd1.ExecuteNonQuery();
                    myclose();
                    return true;
                }
                catch
                {
                    myclose();
                    return false;
                }
            }
            catch
            {
                myclose();
                return false;
            }
        }
        public static bool insertImage(string filePath, int uid)
        {
            string sql = "insert into patient_images(piuid, piImage) values("+uid+",@piImage)";
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            //声明Byte数组
            Byte[] mybyte = new byte[fs.Length];
            
            //读取数据
            fs.Read(mybyte, 0, mybyte.Length);
            fs.Close();
            //转换成二进制数据，并保存到数据库
            MySqlParameter prm = new MySqlParameter("@piImage", MySqlDbType.VarBinary, mybyte.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mybyte);
            
            //MySqlParameter prm1 = new MySqlParameter(
            myopen();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.Parameters.Add(prm);

            try
            {
                cmd.ExecuteNonQuery();
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }

        }
        public static bool updateImage(string filePath, int uid)
        {
            string sql = "update patient_images set piImage=@piImage where piuid=" + uid + ";";
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //声明Byte数组
            Byte[] mybyte = new byte[fs.Length];

            //读取数据
            fs.Read(mybyte, 0, mybyte.Length);
            fs.Close();
            //转换成二进制数据，并保存到数据库
            MySqlParameter prm = new MySqlParameter("@piImage", MySqlDbType.VarBinary, mybyte.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mybyte);

            //MySqlParameter prm1 = new MySqlParameter(
            myopen();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.Parameters.Add(prm);

            try
            {
                cmd.ExecuteNonQuery();
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }

        }
        //直接patient.update(column,value,puid)调用此函数 column为要更改的属性，value为值，puid为user的uid
        //返回bool值
        public static bool Update(string column, string value, int puid)
        {
            myopen();
            string sql = "update patient set " + column + " =";
            if (column == "psex" || column == "page")
            {
                sql += value;
            }
            else
            {
                sql += "'" + value;
            }
            sql += " where puid=" + puid + ";";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
        //pID 代码需重构
        public static bool addDoctor(int uid, int pddid)
        {
            myopen();
            string sql1 = "select pid from patient where puid=" + uid + ";";
            MySqlCommand cmd1 = new MySqlCommand(sql1, con);
            try
            {

                int did = Convert.ToInt16(cmd1.ExecuteScalar());
                string sql = "insert into patient_doctor(pdpid,pddid) values(" + did + "," + pddid + ");";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                myopen();
                cmd.ExecuteNonQuery();
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
        public static bool delete(int uid)
        {
            myopen();
            string sql = "delete from patient_doctor where pdpid=(select pid from patient where puid=" + uid + ");";//AS pd INNER JOIN patient AS p ON p.pid=pd.pdpid AND p.puid=" + uid + ";";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }

    }
    public class user : basicRehab
    {
        public int uID { get; set; }
        public int uSex { get; set; }
        public int uState { get; set; }
        public int uIdentity { get; set; }
        public string uName { get; set; }
        public string uPassword { get; set; }
        public string uEmail { get; set; }
        public string uRname { get; set; }
        public string uAddress { get; set; }
        //dt.date.tostring()
        public DateTime uRegDate { get; set; }
        public DateTime uBirth { get; set; }
        public bool all { get; set; }
        public user()
        {
            
        }

        public List<user> QueryAll()//string[] column, MySqlConnection dd)
        {
            
            List<user> list = new List<user>();
            string sql = "select * from user";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                user us = new user();
                //if (row["id"] != null)
                //{
                us.uID = Convert.ToInt16(row["uid"]);
                us.uName = Convert.ToString(row["uname"]);
                //密码处理
                us.uPassword = Convert.ToString(row["upassword"]);
                if (!row.IsNull("uemail"))
                {
                    us.uEmail = Convert.ToString(row["uemail"]);
                }
                if (!row.IsNull("usex"))
                {
                    us.uSex = Convert.ToByte(row["usex"]);
                }
                if (!row.IsNull("uregdate"))
                {

                    us.uRegDate = Convert.ToDateTime(row["uregdate"]).Date;
                }
                //us.uState = Convert.ToByte(row["ustate"]);
                if (!row.IsNull("uidentity"))
                {
                    us.uIdentity = Convert.ToInt32(row["uidentity"]);
                }
                us.uRname = Convert.ToString(row["urname"]);
                if (!row.IsNull("uaddress"))
                {
                    us.uAddress = Convert.ToString(row["uaddress"]);
                }
                if (!row.IsNull("ubirth"))
                {
                    us.uBirth = Convert.ToDateTime(row["ubirth"]).Date;
                }
                if (!row.IsNull("uall"))
                {
                    us.all = Convert.ToBoolean(row["uall"]);
                }
                
                list.Add(us);
                //}
            }
            return list;
        }
        public List<user> QueryAll(int uid)//string[] column, MySqlConnection dd)
        {

            List<user> list = new List<user>();
            string sql = "select * from user where uid=" + uid +";";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                user us = new user();
                //if (row["id"] != null)
                //{
                us.uID = Convert.ToInt16(row["uid"]);
                us.uName = Convert.ToString(row["uname"]);
                //密码处理
                us.uPassword = Convert.ToString(row["upassword"]);
                if (!row.IsNull("uemail"))
                {
                    us.uEmail = Convert.ToString(row["uemail"]);
                }
                if (!row.IsNull("usex"))
                {
                    us.uSex = Convert.ToByte(row["usex"]);
                }
                if (!row.IsNull("uregdate"))
                {

                    us.uRegDate = Convert.ToDateTime(row["uregdate"]).Date;
                }
                //us.uState = Convert.ToByte(row["ustate"]);
                if (!row.IsNull("uidentity"))
                {
                    us.uIdentity = Convert.ToInt32(row["uidentity"]);
                }
                us.uRname = Convert.ToString(row["urname"]);
                if (!row.IsNull("uaddress"))
                {
                    us.uAddress = Convert.ToString(row["uaddress"]);
                }
                if (!row.IsNull("ubirth"))
                {
                    us.uBirth = Convert.ToDateTime(row["ubirth"]).Date;
                }

                if (!row.IsNull("uall"))
                {
                    us.all = Convert.ToBoolean(row["uall"]);
                }

                list.Add(us);
                //}
            }
            return list;
        }
        //
        public  bool isRight(string us, string pw, out int uid)
        {
            /*string sql1 = "server=localhost;user=root;database=rehab;port=3306;password=root";
            MySqlConnection conn = new MySqlConnection(sql1);
            conn.myopen();*/
            myopen();
            List<user> list = new List<user>();
            string sql = "select upassword from user where uemail='"+us+"';";
            
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                object result = cmd.ExecuteScalar();
                string rpw = Convert.ToString(result);
                if (pw == rpw)
                {
                    myopen();
                    string sql2 = "select uid from user where uemail ='" + us + "';";
                    MySqlCommand cmd1 = new MySqlCommand(sql2, con);
                    try
                    {
                        object result1 = cmd1.ExecuteScalar();
                        uid = Convert.ToInt16(result1);

                        //conn.Close();
                        myclose();
                        return true;
                    }
                    catch
                    {
                        uid = 0;
                        myclose();
                        return false;
                    }
                }
                else
                {
                    uid = 0;
                    //conn.Close();
                    myclose();
                    return false;
                }
            }
            catch
            {
                uid = 0;
                myclose();
                return false;
            }
            
        }

        //verify password is not contain in array rg 
        public bool regist(string[] rg)
        {
            //uname, uemail, upassword
           /* string sql1 = "server=localhost;user=root;database=rehab;port=3306;password=root";
            MySqlConnection conn = new MySqlConnection(sql1);
            conn.myopen();*/
            myopen();
            //use to ustate
            //Byte tem1 = Convert.ToByte(rg[2]);
            string sql = "insert into user(uname, uemail, upassword, uidentity) values('" + rg[0] + "', '" + rg[1] + "', '" + rg[2] + "'," + rg[3]+");";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
            
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            } 
        }
        public bool regist1(string[] rg)
        {
            /*string sql1 = "server=localhost;user=root;database=rehab;port=3306;password=root";
            MySqlConnection conn = new MySqlConnection(sql1);
            conn.myopen();*/
            myopen();
            Byte tem1 = Convert.ToByte(rg[3]);
            DateTime tem2 = Convert.ToDateTime(rg[2]).Date; DateTime regtime = new DateTime();
            regtime = DateTime.Now;
            string sql = "insert into user(urname,ubirth,usex,uaddress,uregdate) valudes('" + rg[0] + "', "  + tem2 + "','" + tem1 + "','" + rg[4] + "','" + regtime+"');";
            
            MySqlCommand cmd = new MySqlCommand(sql, con);

            try
            {
                cmd.ExecuteNonQuery();

                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            } 
        }

        //verify password
        public bool verifyPw(string pw, string npw)
        {
            if (pw != npw)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //verify the format of email
        public bool verifyEmail(string em)
        {
            return isEmail(em);
        }
        public bool checkUnique(string column, string value)
        {
            /*string sql1 = "server=localhost;user=root;database=rehab;port=3306;password=root";
            MySqlConnection conn = new MySqlConnection(sql1);
            conn.myopen();*/
            myopen();
            string sql = "select *  from user where " + column + " ='" +value+"';";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                object re = cmd.ExecuteScalar();
                if (re == null)
                {
                    myclose();
                    return true;
                }
                else
                {
                    myclose();
                    return false;
                }
            }
            catch
            {
                myclose();
                return false;
            }
        }
        //upDate password
        /*public bool upDate(string newPW)
        {
            string sql = "server = localhost;user=root;password=root;port=3306;database=rehab;";
            MySqlConnection conn = new MySqlConnection(sql);
            conn.myopen();
            string sql1 = "update user set upassword='" + newPW + "' where uid=" + uID + ";";
            MySqlCommand cmd = new MySqlCommand(sql1, conn);
            if (cmd.ExecuteNonQuery() == 1)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }*/
        //upDate userinfo
        public bool upDate(string column, string input, int id)
        {
            /*string sql = "server = localhost;user=root;password=root;port=3306;database=rehab;";
            MySqlConnection conn = new MySqlConnection(sql);
            conn.myopen();*/
            myopen();
            string sql = "update user set " + column + " ='" + input + "' where uid=" + id + ";";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                if (column == "uBirth")
                {
                    Convert.ToDateTime(input);
                }
                cmd.ExecuteNonQuery();

                myclose();
                return true;
                
            }
            catch
            {
                myclose();
                return false;
            }
            
        }
        public bool upDate(string column, int input, int id)
        {
            /*string sql = "server = localhost;user=root;password=root;port=3306;database=rehab;";
            MySqlConnection conn = new MySqlConnection(sql);
            conn.myopen();*/
            myopen();
            string sql = "update user set " + column + " =" + input + " where uid=" + id + ";";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
            
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
    }
    public class testtrain : basicRehab
    {
        //the count of actions
        public int tnumbers { get; set; }


        public int ttID { get; set; }
        public int tID { get; set; }
        public int pID { get; set; }
        public int point1 { get; set; }
        public int point2 { get; set; }
        public int point3 { get; set; }
        public int point4 { get; set; }
        public int point5 { get; set; }
        public int point6 { get; set; }
        public int point7 { get; set; }
        public int point8 { get; set; }
        public int point9 { get; set; }
        public int point10 { get; set; }
        public int point { get; set; }
        public DateTime tDate { get; set; }

        public void setNumbers(int temptid)
        {
            myopen();
            string sql = "select tnumbers from train where tid = " + temptid+ ";";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                object result = cmd.ExecuteScalar();
                tnumbers = Convert.ToInt16(result);
                myclose();
            }
            catch
            {
            }
        }

        public List<testtrain> QueryAll(string table)//string[] column, MySqlConnection dd)
        {
            List<testtrain> list = new List<testtrain>();
            string sql = "select * from " + table + ";" ;
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                testtrain us = new testtrain();
                //if (row["id"] != null)
                //{
                us.ttID = Convert.ToInt16(row["ttid"]);
                us.tID = Convert.ToInt16(row["tid"]);
                //密码处理
                us.pID = Convert.ToInt16(row["pid"]);
                if (!row.IsNull("point1"))
                {
                    us.point1 = Convert.ToInt16(row["point1"]);
                }
                if (!row.IsNull("point1"))
                {
                    us.point1 = Convert.ToInt16(row["point1"]);
                } 
                if (!row.IsNull("point2"))
                {
                    us.point2 = Convert.ToInt16(row["point2"]);
                } 
                if (!row.IsNull("point3"))
                {
                    us.point3 = Convert.ToInt16(row["point3"]);
                }
                if (!row.IsNull("point4"))
                {
                    us.point4 = Convert.ToInt16(row["point4"]);
                }
                if (!row.IsNull("point5"))
                {
                    us.point5 = Convert.ToInt16(row["point5"]);
                }
                if (!row.IsNull("point6"))
                {
                    us.point6 = Convert.ToInt16(row["point6"]);
                }
                if (!row.IsNull("point7"))
                {
                    us.point7 = Convert.ToInt16(row["point7"]);
                }
                if (!row.IsNull("point8"))
                {
                    us.point8 = Convert.ToInt16(row["point8"]);
                }
                if (!row.IsNull("point9"))
                {
                    us.point9 = Convert.ToInt16(row["point9"]);
                }
                if (!row.IsNull("point10"))
                {
                    us.point10 = Convert.ToInt16(row["point10"]);
                }
                if (!row.IsNull("point"))
                {
                    us.point = Convert.ToInt16(row["point"]);
                }
                us.tDate = Convert.ToDateTime(row["tdate"]);
                list.Add(us);
                //}
            }
            setNumbers(tID);
            return list;
        }

        // input is a array which has 11 elements;
        public bool insert(string table, int[] input, int pid)
        {
            myopen();
            string sql = "insert into " + table + " set point=" +input[0] +",";
            for (int i = 1; i < input.Length; i++)
            {
                sql += "point" + i + "=" + input[i];
                if (i < input.Length - 1)
                {
                    sql += ",";
                }
                else
                {
                    sql += ",";
                }
            }
            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            sql += "tdate='" + currentTime + "', uid=" + pid+", tid=1;";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
        public List<testtrain> QueryNew(string table, int uid, out int days)
        {
            List<testtrain> re = new List<testtrain>();
            string sql = "select * from " + table + "  where ttid=(select max(ttid) from " + table + ") and uid=" +uid+";";
            testtrain us = new testtrain();
            //if (row["id"] != null)
            //{
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                us.ttID = Convert.ToInt16(row["ttid"]);
                us.tID = Convert.ToInt16(row["tid"]);
                //密码处理
                us.pID = Convert.ToInt16(row["uid"]);
                if (!row.IsNull("point1"))
                {
                    us.point1 = Convert.ToInt16(row["point1"]);
                }
                if (!row.IsNull("point1"))
                {
                    us.point1 = Convert.ToInt16(row["point1"]);
                }
                if (!row.IsNull("point2"))
                {
                    us.point2 = Convert.ToInt16(row["point2"]);
                }
                if (!row.IsNull("point3"))
                {
                    us.point3 = Convert.ToInt16(row["point3"]);
                }
                if (!row.IsNull("point4"))
                {
                    us.point4 = Convert.ToInt16(row["point4"]);
                }
                if (!row.IsNull("point5"))
                {
                    us.point5 = Convert.ToInt16(row["point5"]);
                }
                if (!row.IsNull("point6"))
                {
                    us.point6 = Convert.ToInt16(row["point6"]);
                }
                if (!row.IsNull("point7"))
                {
                    us.point7 = Convert.ToInt16(row["point7"]);
                }
                if (!row.IsNull("point8"))
                {
                    us.point8 = Convert.ToInt16(row["point8"]);
                }
                if (!row.IsNull("point9"))
                {
                    us.point9 = Convert.ToInt16(row["point9"]);
                }
                if (!row.IsNull("point10"))
                {
                    us.point10 = Convert.ToInt16(row["point10"]);
                }
                if (!row.IsNull("point"))
                {
                    us.point = Convert.ToInt16(row["point"]);
                }
                us.tDate = Convert.ToDateTime(row["tdate"]);

                re.Add(us);
            }
            myopen();
            //string sqlDay = "select datediff((select max(tdate) from "+table+"),(select min(tdate) from "+table+"));";
            string sqlDay = "select datediff((select now()),(select min((select tdate from "+table+" where uid="+uid+";)) from " + table + "));";
            MySqlCommand cmd = new MySqlCommand(sqlDay,con);
            try
            {
                object result = cmd.ExecuteScalar();
                days = Convert.ToInt16(result);
                myclose();
                return re;
            }
            catch
            {
                days = 0;
                myclose();
                return null;
            }
        }

        //don't give a too large 'days', it may cause a error; 
        public List<testtrain> QueryDays(string table, int uid, int days)//string[] column, MySqlConnection dd)
        {
            List<testtrain> list = new List<testtrain>();
            string sql = "select * from " + table + " where uid=" + uid + " and DATE_SUB(CURDATE(), INTERVAL "+days+" DAY) <= date(tdate);";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                testtrain us = new testtrain();
                //if (row["id"] != null)
                //{
                us.ttID = Convert.ToInt16(row["ttid"]);
                us.tID = Convert.ToInt16(row["tid"]);
                //密码处理
                us.pID = Convert.ToInt16(row["uid"]);
                if (!row.IsNull("point1"))
                {
                    us.point1 = Convert.ToInt16(row["point1"]);
                }
                if (!row.IsNull("point1"))
                {
                    us.point1 = Convert.ToInt16(row["point1"]);
                }
                if (!row.IsNull("point2"))
                {
                    us.point2 = Convert.ToInt16(row["point2"]);
                }
                if (!row.IsNull("point3"))
                {
                    us.point3 = Convert.ToInt16(row["point3"]);
                }
                if (!row.IsNull("point4"))
                {
                    us.point4 = Convert.ToInt16(row["point4"]);
                }
                if (!row.IsNull("point5"))
                {
                    us.point5 = Convert.ToInt16(row["point5"]);
                }
                if (!row.IsNull("point6"))
                {
                    us.point6 = Convert.ToInt16(row["point6"]);
                }
                if (!row.IsNull("point7"))
                {
                    us.point7 = Convert.ToInt16(row["point7"]);
                }
                if (!row.IsNull("point8"))
                {
                    us.point8 = Convert.ToInt16(row["point8"]);
                }
                if (!row.IsNull("point9"))
                {
                    us.point9 = Convert.ToInt16(row["point9"]);
                }
                if (!row.IsNull("point10"))
                {
                    us.point10 = Convert.ToInt16(row["point10"]);
                }
                if (!row.IsNull("point"))
                {
                    us.point = Convert.ToInt16(row["point"]);
                }
                us.tDate = Convert.ToDateTime(row["tdate"]);

                list.Add(us);
                //}
            }
            return list;
        }

        //a patient can add a doctor in its community
        /*public static bool addDoctor(int duid)
        {
            string sql = "insert into patient_doctor(pdpid,pddid) values(" + pID + "," + duid + ");";
            myopen();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            if (cmd.ExecuteNonQuery() == 1)
            {
                myclose();
                return true;
            }
            else
            {
                myclose();
                return false;
            }
        }*/
    }
    public class doctor : basicRehab
    {
        public int did { get; set;}
        public int duid { get; set; }
        public string drname { get; set; }
        public int dsex { get; set; }
        //public DateTime dbirth { get; set; }
        public int dage { get; set; }
        public string dIDcard { get; set; }
        public string dworkunit { get; set; }
        public string dworkpart { get; set; }
        public string dlevel { get; set; }
        public string dphone { get; set; }
        public string dresume { get; set; }


        //13string members,the last two is the paths of certification and headpic;
        public static bool insert(string[] input)
        {
            myopen();
            
            string sql = "insert into doctor(duid,drname,dsex,dage,dIDcard,dworkunit,dworkpart,"
                + "dlevel,dphone,dresume) values(";
                //+ "dlevel,dphone,dresume) values(";
        
            for (int i = 0; i < input.Length-3; i++)
            {
              
                    if (i==0||i==2||i==3)
                    {
                        sql += input[i] + ",";
                    }
                    else
                    {
                        sql += "'" + input[i] + "',";
                    }
            }
            sql += "'"+input[input.Length-3]+"');";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            /*if (cmd.ExecuteNonQuery() == 1)
            {
                string sql1 = "update user set uall=1 where uid =" + input[0] + ";";
                MySqlCommand cmd1 = new MySqlCommand(sql1, con);
                if (cmd1.ExecuteNonQuery() == 1)
                {
                    myclose();
                    return true;
                }
                else
                {
                    myclose();
                    return false;
                }
            }
            else
            {
                myclose();
                return false;
            }*/
            try
            {
                cmd.ExecuteNonQuery();
                if (!doctor.insertImage(input[input.Length - 2], input[input.Length - 1], Convert.ToInt16(input[0])))
                {
                    myclose();
                    return false;
                }
                string sql1 = "update user set uall=1 where uid =" + input[0] + ";";
                MySqlCommand cmd1 = new MySqlCommand(sql1, con);
                try
                {
                    myopen();
                    cmd1.ExecuteNonQuery();

                    myclose();
                    return true;
                }

                catch
                {
                    myclose();
                    return false;
                }

            }
            catch
            {
                myclose();
                return false;
            }
        }
        public bool Update(string column, string value)
        {
            myopen();
            string sql = "update doctor set "+column+" =";
            if (column == "dsex" || column == "dage")
            {
                sql += value;
            }
            else
            {
                sql += "'" + value + "'";
            }
            sql += " where duid=" + duid + ";"; 
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();         
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
        
        public List<doctor> queryAll(string table, string state="", string columns="*")
        {
            List<doctor> list = new List<doctor>();
            string sql = "select " + columns + " from " + table;
            if (state != "")
            {
                sql += " where " + state;
            }
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                doctor temp = new doctor();
                temp.did = Convert.ToInt16(row["did"]);
                temp.duid = Convert.ToInt16(row["duid"]);
                temp.drname = Convert.ToString(row["drname"]);
                temp.dsex = Convert.ToInt16(row["dsex"]);
                temp.dage = Convert.ToInt16(row["dage"]);
                temp.dIDcard = Convert.ToString(row["dIDcard"]);
                temp.dworkunit = Convert.ToString(row["dworkunit"]);
                temp.dworkpart = Convert.ToString(row["dworkpart"]);
                temp.dlevel = Convert.ToString(row["dlevel"]);
                temp.dphone = Convert.ToString(row["dphone"]);
                temp.dresume = Convert.ToString(row["dresume"]);
                list.Add(temp);
            }
            return list;
        }
        public static List<patient> queryPatient(int duid)
        {
            List<patient> list = new List<patient>();
            string sql = "select p.* from patient AS p INNER JOIN patient_doctor AS pd INNER JOIN doctor AS d ON d.did=pd.pddid AND d.duid="+duid+";";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                patient temp = new patient();
                temp.pID = Convert.ToInt16(row["pid"]);
                temp.puid = Convert.ToInt16(row["puid"]);
                temp.pSex = Convert.ToInt16(row["psex"]);
                temp.pAge = Convert.ToInt16(row["page"]);
                temp.pCareer = Convert.ToString(row["pcareer"]);
                temp.pPhone = Convert.ToString(row["pphone"]);
                temp.pEmail = Convert.ToString(row["pemail"]);
                temp.pWorkUnit = Convert.ToString(row["pworkunit"]);
                temp.pConAddress = Convert.ToString(row["pconaddress"]);
                temp.pOpHistory = Convert.ToString(row["pophistory"]);
                temp.pAllergy = Convert.ToString(row["pallergy"]);
                temp.pInjuredPart = Convert.ToString(row["pinjuredpart"]);
                temp.pHospital = Convert.ToString(row["phospital"]);
                temp.pDiagnosis = Convert.ToString(row["pdiagnosis"]);
                temp.pInspectlist = Convert.ToString(row["pinspectlist"]);
                temp.pRname = Convert.ToString(row["prname"]);
                list.Add(temp);
            }
            return list;
        }
        public static bool insertImage(string filePath1, string filePath2, int uid)
        {
            string sql = "insert into doctor_images(diuid, diHeadImage,diCertificationimage) values("+uid+",@diHeadImage,@diCertification)";
            FileStream fs1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read);
            //声明Byte数组
            Byte[] mybyte1 = new byte[fs1.Length];
            Byte[] mybyte2 = new byte[fs2.Length];
            //读取数据
            fs1.Read(mybyte1, 0, mybyte1.Length);
            fs1.Close();
            fs2.Read(mybyte2, 0, mybyte2.Length);
            fs2.Close();
            //转换成二进制数据，并保存到数据库
            MySqlParameter prm1 = new MySqlParameter("@diHeadImage", MySqlDbType.VarBinary, mybyte1.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mybyte1);
            MySqlParameter prm2 = new MySqlParameter("@diCertification", MySqlDbType.VarBinary, mybyte2.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mybyte2);
            //MySqlParameter prm1 = new MySqlParameter(
            myopen();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.Parameters.Add(prm1);
            cmd.Parameters.Add(prm2);
            try
            {
                cmd.ExecuteNonQuery();
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
        public static BitmapImage[] getImage(int id)
        {
             //string strConn = "server=111.67.201.113;database=rehab;uid=root;pwd=85589890";
            //string strConn = conSQL;
            //创建SqlConnection对象
            myopen();
            //打开数据库连接
            //创建SQL语句
            string sql = "select diHeadImage,diCertificationImage from doctor_images where diuid="+id+";";
            //string sql = "select dicertificationimage from doctor_images";
            //string sql = "select blobdata from images;";
            //创建SqlCommand对象
            MySqlCommand command = new MySqlCommand(sql, con);
            //创建DataAdapter对象
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
            //创建DataSet对象
            DataSet dataSet = new DataSet();
            DataTable dt = new DataTable();
            dt.TableName = "doctor_images";
            //dt.TableName = "images";
            dataAdapter.Fill(dataSet, "doctor_images");
            //dataAdapter.Fill(dataSet, "images");
            int c = dataSet.Tables["doctor_images"].Rows.Count;
            //int c = dataSet.Tables["images"].Rows.Count;
            if (c > 0)
            {
                Byte[] mybyte1 = new byte[0];
                Byte[] mybyte2 = new byte[0];

                mybyte1 = (Byte[])(dataSet.Tables["doctor_images"].Rows[c - 1]["diheadimage"]);
                //mybyte1 = (Byte[])(dataSet.Tables["images"].Rows[c - 1]["blobdata"]);
                mybyte2 = (Byte[])(dataSet.Tables["doctor_images"].Rows[c - 1]["diCertificationImage"]);
                MemoryStream ms1 = new MemoryStream(mybyte1);
                MemoryStream ms2 = new MemoryStream(mybyte2);
                System.Drawing.Image my1 = System.Drawing.Image.FromStream(ms1);
                System.Drawing.Image my2 = System.Drawing.Image.FromStream(ms2);
                BitmapImage image1 = new BitmapImage();
                BitmapImage image2 = new BitmapImage();
                //以流的形式初始化图片
                image1.BeginInit();
                image2.BeginInit();
                image1.StreamSource = new MemoryStream(mybyte1);
                image2.StreamSource = new MemoryStream(mybyte2);
                image1.EndInit();
                image2.EndInit();
                BitmapImage[] re = new BitmapImage[2];
                re[0] = image1;
                re[1] = image2;
                myclose();
                return re;
                //myImage1.Source = image1;
                //myImage2.Source = image2;

                //     MyImage.Width = image.PixelWidth;
                //   MyImage.Height = image.PixelHeight;
                // MyImage = new System.Windows.Controls.Image();
                //MyBorder.Child = System.Drawing.Image.FromStream(ms);
                //= Image.FromStream(ms);
            }
            else
            {
                myclose();
                return null;
            }
                //myImage1.Source = image1;
                //myImage2.Source = image2;

                //     MyImage.Width = image.PixelWidth;
                //   MyImage.Height = image.PixelHeight;
                // MyImage = new System.Windows.Controls.Image();
                //MyBorder.Child = System.Drawing.Image.FromStream(ms);
                //= Image.FromStream(ms);
            
            
        }
        public static bool updateImage(string filePath1, string filePath2, int uid)
        {
            string sql = "update doctor_images set diHeadImage=@diHeadImage,diCertificationImage=@diCertificationImage where diuid="+uid+";";
            FileStream fs1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read);
            //声明Byte数组
            Byte[] mybyte1 = new byte[fs1.Length];
            Byte[] mybyte2 = new byte[fs2.Length];
            //读取数据
            fs1.Read(mybyte1, 0, mybyte1.Length);
            fs1.Close();
            fs2.Read(mybyte2, 0, mybyte2.Length);
            fs2.Close();
            //转换成二进制数据，并保存到数据库
            MySqlParameter prm1 = new MySqlParameter("@diHeadImage", MySqlDbType.VarBinary, mybyte1.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mybyte1);
            MySqlParameter prm2 = new MySqlParameter("@diCertificationImage", MySqlDbType.VarBinary, mybyte2.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, mybyte2);
            //MySqlParameter prm1 = new MySqlParameter(
            myopen();
            MySqlCommand cmd = new MySqlCommand(sql, con);
            cmd.Parameters.Add(prm1);
            cmd.Parameters.Add(prm2);
            try
            {
                cmd.ExecuteNonQuery();
            
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
        //public void 
        //duid为医生的uid,  pid为病人的pid
        public static bool delete(int duid, int pid)
        {
            myopen();
            string sql = "delete from patient_doctor AS pd INNER JOIN patient AS p INNER JOIN doctor AS d ON pd.pddid=d.did  AND d.duid=" + duid + " AND p.pid="+pid+";";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
            
                myclose();
                return true;
            }
            catch
            {
                myclose();
                return false;
            }
        }
    }
    public class message : basicRehab
    {
        public int mid { get; set; }
        public int mpuid { get; set; }
        public int mduid { get; set; }
        public int mdirec { get; set; }
        public DateTime mdate { get; set; }
        public string mtext { get; set; }
        public int mreplied { get; set; }
        public string mdname { get; set; }
        public string mpname { get; set; }
        //direc:所回复messageID
        public static bool insert(int puid, int duid, string text, int direc=0)
        {
            myopen();
            DateTime date = new DateTime();
            date = DateTime.Now;
            string tdate;
            tdate = date.ToString("yyyy-MM-dd hh:mm:ss");
            string sql = "insert into message(mpuid,mduid,mtext,mdate,mdirec) values(" + puid + "," + duid + ",'" + text + "','" + tdate +"',"+direc+ ");";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            try
            {
                cmd.ExecuteNonQuery();
            
                if (direc != 0)
                {
                    string sql1 = "update message set mreplied=1 where mid="+direc+";";
                    MySqlCommand cmd1 = new MySqlCommand(sql1, con);
                    try
                    {
                        cmd1.ExecuteNonQuery();
                    
                        myclose();
                        return true;
                    }
                    catch
                    {
                        myclose();
                        return false;
                    }
                }
                else
                {
                    myclose();
                    return true;
                }
                
            }
            catch
            {
                myclose();
                return false;
            }
        }
        public static List<message> queryAll(int doctorUID, int patientUID)
        {
            List<message> list = new List<message>();
            //string sql = "select * from message where mduid=" + doctorUID +" AND puid="+ patientUID +";";
            string sql = "select * from message AS m INNER JOIN doctor AS d INNER JOIN patient AS p ON m.mduid=d.duid AND m.mpuid=p.puid AND m.mduid=" + doctorUID + " AND mpuid=" + patientUID + ";";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                message temp = new message();
                temp.mid = Convert.ToInt16(row["mid"]);
                temp.mpuid = Convert.ToInt16(row["mpuid"]);
                temp.mduid = Convert.ToInt16(row["mduid"]);
                temp.mtext = Convert.ToString(row["mtext"]);
                temp.mdirec = Convert.ToInt16(row["mdirec"]);
                temp.mreplied = Convert.ToInt16(row["mreplied"]);
                temp.mdname = Convert.ToString(row["drname"]);
                temp.mpname = Convert.ToString(row["prname"]);
                list.Add(temp);
            }
            return list;
        }
        public static List<message> queryAll(int doctorUID)
        {
            List<message> list = new List<message>();
            string sql = "select * from message AS m INNER JOIN doctor AS d INNER JOIN patient AS p ON m.mduid=d.duid AND m.mpuid=p.puid AND m.mduid=" + doctorUID + ";";
            foreach (DataRow row in MySqlHelper.ExecuteDataTable(conSQL, sql, null).Rows)
            {
                message temp = new message();
                temp.mid = Convert.ToInt16(row["mid"]);
                temp.mpuid = Convert.ToInt16(row["mpuid"]);
                temp.mduid = Convert.ToInt16(row["mduid"]);
                temp.mtext = Convert.ToString(row["mtext"]);
                temp.mdirec = Convert.ToInt16(row["mdirec"]);
                temp.mreplied = Convert.ToInt16(row["mreplied"]);
                temp.mdname = Convert.ToString(row["drname"]);
                temp.mpname = Convert.ToString(row["prname"]);
                list.Add(temp);
            }
            return list;
        }
    }
}
