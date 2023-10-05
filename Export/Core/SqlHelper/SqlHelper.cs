

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;
using System.Xml;
using System.Configuration;

namespace Core.SqlHelper
{
    public class SqlHelper
    {
        private  string mSQLConnectionString; // = ReadConnectionString()
        private  string mOLEDBConnectionString;
        private  SqlConnection mConnection;
        private  SqlTransaction mCurrentTrans = null;
        private  SqlCommand mSQLCommand;
        private  int mBeginTransTimes = 0;
        // Private Shared mInTrans As Boolean = False
        // 
        // DatabaseServer Information
        public  string DB_SERVER;
        public  string DB_DATABASE;
        public  string DB_USER;

        public SqlHelper()
        {
        }
        // Public Sub New(ByVal Conn As SqlConnection)
        // mConnection = Conn
        // Me.mSQLConnectionString = Conn.ConnectionString
        // End Sub
        public SqlHelper(string strConn)
        {
            mSQLConnectionString = strConn;
            mConnection = new SqlConnection();
            mConnection.ConnectionString = strConn;
        }

        public  SqlConnection Connection
        {
            get
            {
                if ((mConnection.State != ConnectionState.Open))
                {
                    mConnection.ConnectionString = mSQLConnectionString;
                    mConnection.Open();
                }
                return mConnection;
            }
            set
            {
                mConnection = Connection;
                mSQLConnectionString = mConnection.ConnectionString;
            }
        }

        public  string ConnectionString
        {
            get
            {
                return mSQLConnectionString;
            }
        }

        public  string OLEDBConnectionString
        {
            get
            {
                return mOLEDBConnectionString;
            }
        }

        public  bool InTransaction
        {
            get
            {
                if (mCurrentTrans != null)
                    return true;
                else
                    return false;
            }
        }
      

        // Kết nối máy chủ SQL SERVER thông qua thiết lập của hệ thống
        public  bool Connect(bool bRefreshConnect = false)
        {
            try
            {
                mConnection = new SqlConnection(mSQLConnectionString);
                // Mở kết nối
                mConnection.Open();
                SetGlobalSqlLanguage();
                return true;
            }
            catch (Exception ex)
            {
                mConnection = null;
                return false;
            }
        }

        public  SqlConnection Connect(string dbHost, string dbDatabase, string dbUserID, string dbPassword, bool WindowsAuthentication = false)
        {
            SqlConnection nCnn;
            string nCnnString;
            try
            {
                // 
                if (WindowsAuthentication)
                    nCnnString = "Data Source=" + dbHost + ";Initial Catalog=" + dbDatabase + ";Integrated Security=SSPI;MultipleActiveResultSets=True;pooling=true;Max Pool Size=1000000;Connect Timeout=300000; " + "Persist Security info=False;" + "Trusted_Connection=Yes;";
                else
                    nCnnString = "Data Source=" + dbHost
                                   + "; Initial Catalog=" + dbDatabase
                                   + "; User ID=" + dbUserID
                                   + ";PWD=" + dbPassword
                                   + ";MultipleActiveResultSets=True;pooling=true;Max Pool Size=1000000;Connect Timeout=300000; ";
                // Mở kết nối
                nCnn = new SqlConnection(nCnnString);
                nCnn.CreateCommand().CommandTimeout = 1000;
                nCnn.Open();
                return nCnn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // Public Shared Sub SetConnectionString(ByVal Connstring As String)
        // mSQLConnectionString = Connstring
        // End Sub
        // Public Shared Function ReadConnectionString() As String
        // Try
        // Dim reg As New cadsRegistry
        // mSQLConnectionString = reg.GetReg("ConnectionString")
        // Return mSQLConnectionString
        // Catch ex As Exception
        // MsgBox(cadsVar.msgERRO & ex.Message)
        // End Try
        // End Function

        // Public Shared Function Load_Connection(ByRef toConnection As SqlClient.SqlConnection, Optional ByVal ServerName As String = "localhost", Optional ByVal DBName As String = "CADS", Optional ByVal UserName As String = "sa", Optional ByVal PWD As String = "") As Boolean
        // Dim sConnString As String = "Data Source=" & ServerName & _
        // ";Initial Catalog=" & DBName & _
        // ";User ID=" & UserName & _
        // ";Password=" & PWD
        // Try
        // toConnection = New SqlConnection(sConnString)
        // toConnection.Open()
        // mConnection = toConnection
        // Return True
        // Catch ex As Exception
        // MsgBox("" & ex.Message)
        // Return False
        // End Try
        // End Function

        public  int Execute(string strSQL)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    int result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();
                    // Trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 0;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;

                    int result = mSQLCommand.ExecuteNonQuery();
                    // Trả về giá trị
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  int Execute(string strSQL, SqlParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL
                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    int result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();
                    // Trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 0;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    int result = mSQLCommand.ExecuteNonQuery();
                    // Trả về giá trị
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  int Execute(string strSQL, CoreParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL
                    mSQLCommand = new SqlCommand(strSQL, mConnection);

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));

                    int result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();
                    // Trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 0;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    // Các tham số

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));


                    int result = mSQLCommand.ExecuteNonQuery();
                    // Trả về giá trị
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  int Execute(string strSQL, CoreParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // 
                    // Tạo câu lệnh SQL
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }
                    int result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();

                    // Trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 0;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    // Các tham số
                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    int result = mSQLCommand.ExecuteNonQuery();
                    // Trả về giá trị
                    return result;
                }
            }

            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public int ExecuteStore(string strStoreName, CoreParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // 
                    // Tạo câu lệnh SQL
                    mSQLCommand = new SqlCommand(strStoreName, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }
                    int result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();

                    // Trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 0;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = strStoreName;
                    mSQLCommand.Parameters.Clear();

                    // Các tham số
                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    int result = mSQLCommand.ExecuteNonQuery();
                    // Trả về giá trị
                    return result;
                }
            }

            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  DataSet LoadDataSet(string strSQL)
        {
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    mConnection = new SqlConnection(mSQLConnectionString);
                    // Mở kết nối
                    mConnection.Open();
                }
                SqlDataAdapter adpt = new SqlDataAdapter(strSQL, mConnection);

                DataSet ds = new DataSet();
                adpt.Fill(ds);
                mConnection.Close();
                return ds;
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  DataTable LoadTableSchema(string TableName)
        {
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    mConnection = new SqlConnection(mSQLConnectionString);
                    // Mở kết nối
                    mConnection.Open();
                }
                DataTable dt = new DataTable();
                System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM " + TableName + " WHERE 1=0", mConnection);
                da.FillSchema(dt, SchemaType.Mapped);
                mConnection.Close();
                return dt;
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  DataTable LoadTable(string strSQL)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    mConnection.Close();
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  DataTable LoadTable(string strSQL, SqlParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    mConnection.Close();
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  DataTable LoadTable(string strSQL, CoreParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    mConnection.Close();
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  DataTable LoadTable(string strSQL, CoreParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    mConnection.Close();
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public DataTable LoadTableFromStoreProcedure(string storeName, CoreParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(storeName, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    mConnection.Close();
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = storeName;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  object ExecuteScalar(string strSQL)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    object result = mSQLCommand.ExecuteScalar();
                    mConnection.Close();
                    // trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;

                    object result = mSQLCommand.ExecuteScalar();
                    // trả về giá trị
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  object ExecuteScalar(string strSQL, CoreParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(strSQL, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));

                    object result = mSQLCommand.ExecuteScalar();
                    mConnection.Close();
                    // trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    SqlParameter SqlPara = new SqlParameter(param.Name, param.Value);
                    mSQLCommand.Parameters.Add(SqlPara);
                    object result = mSQLCommand.ExecuteScalar();
                    // trả về giá trị
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  object ExecuteScalar(string strSQL, CoreParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(strSQL, mConnection);

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        SqlParameter SqlPara = new SqlParameter(param[i].Name, param[i].Value);
                        mSQLCommand.Parameters.Add(SqlPara);
                    }

                    object result = mSQLCommand.ExecuteScalar();
                    mConnection.Close();
                    // trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.Text;
                    mSQLCommand.CommandText = strSQL;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        SqlParameter SqlPara = new SqlParameter(param[i].Name, param[i].Value);
                        mSQLCommand.Parameters.Add(SqlPara);
                    }

                    object result = mSQLCommand.ExecuteScalar();
                    // trả về giá trị
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
        }

        public  int ExecuteSP(string storedProcedure)
        {
            int result;
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL

                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();

                }
                else
                {
                    // Tạo câu lệnh SQL
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    result = mSQLCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
            return result;
        }

        public  int ExecuteSP(string storedProcedure, CoreParameter param)
        {
            int result;
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    // Nếu chưa có kết nối thì tạo kết nối
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL

                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    SqlParameter SqlPara = new SqlParameter(param.Name, param.Value);
                    mSQLCommand.Parameters.Add(SqlPara);
                    result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();
                }
                else
                {
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    SqlParameter SqlPara = new SqlParameter(param.Name, param.Value);
                    mSQLCommand.Parameters.Add(SqlPara);

                    result = mSQLCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
            return result;
        }

        public  int ExecuteSP(string storedProcedure, SqlParameter param)
        {
            int result;
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    // Nếu chưa có kết nối thì tạo kết nối
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL

                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    result = mSQLCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
            return result;
        }

        public  int ExecuteSP(string storedProcedure, CoreParameter[] param)
        {
            int result;
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL

                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    result = mSQLCommand.ExecuteNonQuery();
                    mConnection.Close();
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    result = mSQLCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                if (mConnection.State == ConnectionState.Open && mBeginTransTimes <= 0) mConnection.Close();
                throw ex;
            }
            return result;
        }

        public  int ExecuteSP(string storedProcedure, SqlParameter[] param)
        {
            int result;
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh SQL

                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(param[i]);
                    }

                    result = mSQLCommand.ExecuteNonQuery();
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(param[i]);
                    }

                    result = mSQLCommand.ExecuteNonQuery();
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return result;
        }

        public  object ExecuteScalarSP(string storedProcedure)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    object result = mSQLCommand.ExecuteScalar();
                    // trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    object result = mSQLCommand.ExecuteScalar();
                    // trả về giá trị
                    return result;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  object ExecuteScalarSP(string storedProcedure, CoreParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    object result = mSQLCommand.ExecuteScalar();
                    // mConnection.Close()
                    // trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));

                    object result = mSQLCommand.ExecuteScalar();
                    // trả về giá trị
                    return result;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  object ExecuteScalarSP(string storedProcedure, CoreParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    object result = mSQLCommand.ExecuteScalar();
                    // mConnection.Close()
                    // trả về giá trị
                    return result;
                }
                else
                {
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    object result = mSQLCommand.ExecuteScalar();
                    // trả về giá trị
                    return result;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  DataTable SelectTableSP(string storedProcedure)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // mConnection.Close()
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = storedProcedure;

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  DataTable SelectTableSP(string storedProcedure, CoreParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // mConnection.Close()
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(new SqlParameter(param.Name, param.Value));

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  DataTable SelectTableSP(string storedProcedure, SqlParameter param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // mConnection.Close()
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    // Tạo câu lệnh truy vấn
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    if (param.Value == null)
                        param.Value = DBNull.Value;
                    mSQLCommand.Parameters.Add(param);

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  DataTable SelectTableSP(string storedProcedure, CoreParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // mConnection.Close()
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(new SqlParameter(param[i].Name, param[i].Value));
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  DataTable SelectTableSP(string storedProcedure, SqlParameter[] param)
        {
            try
            {
                if (mBeginTransTimes <= 0)
                {
                    if (mConnection.State != ConnectionState.Open)
                    {
                        mConnection = new SqlConnection(mSQLConnectionString);
                        // Mở kết nối
                        mConnection.Open();
                    }
                    // Tạo câu lệnh truy vấn
                    mSQLCommand = new SqlCommand(storedProcedure, mConnection);
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandType = CommandType.StoredProcedure;

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(param[i]);
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // mConnection.Close()
                    // trả về giá trị
                    return dt;
                }
                else
                {
                    mSQLCommand.CommandType = CommandType.StoredProcedure;
                    mSQLCommand.CommandTimeout = 300;
                    mSQLCommand.CommandText = storedProcedure;
                    mSQLCommand.Parameters.Clear();

                    int i;
                    for (i = 0; i <= param.Length - 1; i++)
                    {
                        if (param[i].Value == null)
                            param[i].Value = DBNull.Value;
                        mSQLCommand.Parameters.Add(param[i]);
                    }

                    // Tạo Adapter để lấy dữ liệu
                    SqlDataAdapter da = new SqlDataAdapter(mSQLCommand);
                    // Fill vào bảng 
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    // trả về giá trị
                    return dt;
                }
            }
            // Catch se As SqlException
            // 'If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
            // cadsLog.SaveException(se)
            // MsgBox(cadsVar.msgERRO & se.Message)
            catch (Exception ex)
            {
                // If (mConnection.State = ConnectionState.Open) And (mBeginTransTimes <=0) Then mConnection.Close()
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
            return null;
        }

        public  System.Data.SqlClient.SqlDataAdapter InitAdapter(string TableName)
        {
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    mConnection = new SqlConnection(mSQLConnectionString);
                    // Mở kết nối
                    mConnection.Open();
                }
                System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter("SELECT * FROM " + TableName, mConnection);
                System.Data.SqlClient.SqlCommandBuilder cmdBuilder = new System.Data.SqlClient.SqlCommandBuilder(adp);
                cmdBuilder.QuotePrefix = "[";
                cmdBuilder.QuoteSuffix = "]";
                adp.InsertCommand = cmdBuilder.GetInsertCommand();
                adp.UpdateCommand = cmdBuilder.GetUpdateCommand();
                adp.DeleteCommand = cmdBuilder.GetDeleteCommand();
                return adp;
            }
            catch (Exception ex)
            {
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message); // MsgBox(cadsVar.msgERRO & ex.Message)
                return null;
            }
        }

        public  System.Data.SqlClient.SqlDataAdapter InitAdapter(string Fields, string TableName)
        {
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    mConnection = new SqlConnection(mSQLConnectionString);
                    // Mở kết nối
                    mConnection.Open();
                }
                System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter("SELECT " + Fields + " FROM " + TableName, mConnection);
                System.Data.SqlClient.SqlCommandBuilder cmdBuilder = new System.Data.SqlClient.SqlCommandBuilder(adp);
                cmdBuilder.QuotePrefix = "[";
                cmdBuilder.QuoteSuffix = "]";
                adp.InsertCommand = cmdBuilder.GetInsertCommand();
                adp.UpdateCommand = cmdBuilder.GetUpdateCommand();
                adp.DeleteCommand = cmdBuilder.GetDeleteCommand();
                return adp;
            }
            catch (Exception ex)
            {
                // cadsLog.SaveException(ex)
                throw new Exception(ex.Message); // MsgBox(cadsVar.msgERRO & ex.Message)
                return null;
            }
        }


        public  void BeginTransaction()
        {
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    mConnection = new SqlConnection(mSQLConnectionString);
                    // Mở kết nối
                    mConnection.Open();
                }
                // Gắn kết nối với Command này
                mSQLCommand = new SqlCommand();
                mSQLCommand.Connection = mConnection;
                // Chuẩn bị transaction
                if (mCurrentTrans == null)
                    mCurrentTrans = mConnection.BeginTransaction();

                if (mBeginTransTimes < 0)
                    mBeginTransTimes = 1;
                else
                    mBeginTransTimes += 1;
                mSQLCommand.Transaction = mCurrentTrans;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }// MsgBox(cadsVar.msgERRO & ex.Message)
        }

        public  void Commit()
        {

            try
            {
                if ((mCurrentTrans != null))
                {
                    mCurrentTrans.Commit();
                    mCurrentTrans = null;
                }
            }
            catch
            {
                mCurrentTrans = null;
                throw;
            }
        }

        public  void RollBack()
        {

            try
            {
                if ((mCurrentTrans != null))
                {
                    mCurrentTrans.Rollback();
                    mCurrentTrans = null;
                }
            }
            catch
            {
                mCurrentTrans = null;
                throw;
            }
        }


        // ----------------------------------------------------------------------


        // Thiết lập kiểu DateTime cho SQL Connection
        public  void SetGlobalSqlLanguage(SqlConnection nConnection = null)
        {
            string nSQLDatePatern, sShortDateParten;
            string[] nDatePatern;
            try
            {
                // Set SQL Date
                string cDateSeparator = "";
                sShortDateParten = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToString();

                foreach (char c in sShortDateParten)
                {
                    if (c.ToString().ToLower() != "d" && c.ToString().ToUpper() != "M" && c.ToString().ToLower() != "y")
                    {
                        cDateSeparator = c.ToString();
                        break;
                    }
                }
                // 
                nDatePatern = sShortDateParten.Split(cDateSeparator[0]);

                if ((nDatePatern[0].ToUpper() == "D" | nDatePatern[0].ToUpper() == "DD") & (nDatePatern[1].ToUpper() == "M" | nDatePatern[1].ToUpper() == "MM"))
                    // SET LANGUAGE FRENCH
                    // dd/MM/yyyy
                    nSQLDatePatern = "SET DATEFORMAT dmy"; // "SET LANGUAGE FRENCH"
                else if ((nDatePatern[0].ToUpper() == "M" | nDatePatern[0].ToUpper() == "MM") & (nDatePatern[1].ToUpper() == "D" | nDatePatern[1].ToUpper() == "DD"))
                    // SET LANGUAGE ENGLISH 
                    // MM/dd/yyyy
                    nSQLDatePatern = "SET DATEFORMAT mdy"; // "SET LANGUAGE ENGLISH "
                else if ((nDatePatern[1].ToUpper() == "M" | nDatePatern[1].ToUpper() == "MM")
                                 & (nDatePatern[2].ToUpper() == "D" | nDatePatern[2].ToUpper() == "DD"))
                    // SET LANGUAGE JAPANESE
                    // yyyy/MM/dd
                    nSQLDatePatern = "SET DATEFORMAT ymd"; // "SET LANGUAGE JAPANESE"
                else if ((nDatePatern[1].ToUpper() == "D" | nDatePatern[1].ToUpper() == "DD") & (nDatePatern[2].ToUpper() == "M" | nDatePatern[2].ToUpper() == "MM"))
                    // SET LANGUAGE ydm
                    // yyyy/dd/MM
                    nSQLDatePatern = "SET DATEFORMAT ydm"; // "SET LANGUAGE ydm"
                else
                    nSQLDatePatern = "";
                if (nSQLDatePatern != "")
                {
                    SqlCommand f_command;
                    if (nConnection != null)
                        f_command = new System.Data.SqlClient.SqlCommand(nSQLDatePatern, nConnection);
                    else
                        f_command = new System.Data.SqlClient.SqlCommand(nSQLDatePatern, mConnection);
                    f_command.ExecuteNonQuery();
                    f_command.Dispose();
                }
            }
            catch (Exception ex)
            {
                //cadsLog.SaveException(ex);
                // MsgBox(cadsVar.msgERRO + ex.Message);
            }
        }

        // Shared Function InvisibledKeys(ByVal KeyName As String, Optional ByVal strFromDate As String = "", Optional ByVal strToDate As String = "") As String
        // Dim strSQL As String
        // If strFromDate = "" Then strFromDate = cadsVar.gDateStart.ToShortDateString
        // If strToDate = "" Then strToDate = cadsVar.gDateEnd.ToShortDateString
        // strSQL = "SELECT KEY_VALUE FROM SYS_InvisibledKeys" _
        // & " WHERE KEY_NAME=N'" & KeyName & "'" _
        // & " AND USER_ID=" & cadsVar.gUserID _
        // & " AND ID_DVCS=" & cadsVar.gID_dvcs _
        // & " AND DATE_FROM >='" & strFromDate & "'" _
        // & " AND DATE_TO <='" & strToDate & "'"
        // Return strSQL
        // End Function

        public  string CStrIN(string strINPUT)
        {
            strINPUT = strINPUT.Replace("'", "''");
            strINPUT = strINPUT.Replace(",", "','");
            strINPUT = "'" + strINPUT + "'";
            return strINPUT;
        }

        // Shared Function RecCount(ByVal strFilter As String, ByVal dtData As DataTable, Optional ByRef arrRows() As DataRow = Nothing) As Integer
        // Try
        // arrRows = dtData.Select(strFilter)
        // If arrRows Is Nothing Then
        // Return -1
        // Else
        // Return arrRows.Length
        // End If
        // Catch ex As Exception
        // cadsLog.SaveException(ex)
        // MsgBox(cadsVar.msgERRO & ex.Message)
        // Return -1
        // End Try
        // End Function

        public  string Degist(string data)
        {
            // Public Shared Function encryptData(ByVal data As String) As Byte()
            string str = "";
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            foreach (byte b in hashedBytes)
                // Chuyển các byte sang dạng chuỗi
                str += b.ToString("x2");
            return str;
        }

        protected void Dispose()
        {
            mSQLConnectionString = null;
            mOLEDBConnectionString = null;
            mConnection = null;
            mCurrentTrans = null;
            mSQLCommand = null;
            // 
            // DatabaseServer Information
            DB_SERVER = null;
            DB_DATABASE = null;
            DB_USER = null;
        }

    }

    public class CoreParameter
    {
        private string name;
        private object _value;
        public CoreParameter()
        {
            this.name = "";
            this._value = null;
        }
        public CoreParameter(string name, object value, bool SetNULLvalue = false)
        {
            this.name = name;
            if (value == null || value == null || SetNULLvalue)
                this._value = System.DBNull.Value;
            else
                // Me.value = value
                if ((value) is DateTime)
            {
                if (((DateTime)value).Day <= 1 && ((DateTime)value).Month <= 1 && ((DateTime)value).Year <= 1753)
                    this._value = System.DBNull.Value;
                else
                    this._value = value;
            }
            else
                this._value = value;
        }
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                // Me.value = Value
                if (_value != null && value != null)
                {
                    if ((Value) is DateTime)
                    {
                        if (((DateTime)value).Year <= 1753)
                            this._value = System.DBNull.Value;
                        else
                            this._value = Value;
                    }
                    else
                        this._value = Value;
                }
                else
                    this._value = System.DBNull.Value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }


    }

    public class DBInfo 
    {
        private string dbServer = string.Empty;
        public string DBServer
        {
            get
            {
                return dbServer;
            }
            set
            {
                dbServer = value;
            }
        }
        private string dbDatabase = string.Empty;
        public string DBDatabase
        {
            get
            {
                return dbDatabase;
            }
            set
            {
                dbDatabase = value;
            }
        }
        private string dbUser = string.Empty;
        public string DBUser
        {
            get
            {
                return dbUser;
            }
            set
            {
                dbUser = value;
            }
        }
        private string dbPassword = string.Empty;
        public string DBPassword
        {
            get
            {
                return dbPassword;
            }
            set
            {
                dbPassword = value;
            }
        }
        private bool windowsAuthentication = false;
        public bool WindowsAuthentication
        {
            get
            {
                return windowsAuthentication;
            }
            set
            {
                windowsAuthentication = value;
            }
        }
    }

}