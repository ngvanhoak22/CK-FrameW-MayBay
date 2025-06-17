using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

public class DataContext
{
    private readonly string _connectionString;

    public DataContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Thêm hành khách
    public void AddHanhKhach(HanhKhach hanhKhach)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "INSERT INTO HANHKHACH (MAHK, HOTEN, DIACHI, DIENTHOAI) VALUES (@MAHK, @HOTEN, @DIACHI, @DIENTHOAI)";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MAHK", hanhKhach.MAHK);
                command.Parameters.AddWithValue("@HOTEN", hanhKhach.HOTEN);
                command.Parameters.AddWithValue("@DIACHI", hanhKhach.DIACHI);
                command.Parameters.AddWithValue("@DIENTHOAI", hanhKhach.DIENTHOAI);
                command.ExecuteNonQuery();
            }
        }
    }

    // Lấy thông tin chuyến bay theo mã
    public ChuyenBayViewModel GetChuyenBay(string maChuyen)
    {
        var hanhKhachs = new List<CT_CB_HK>();
        var ChuyenBay = new ChuyenBay();
        int thuong = 0;
        int vip = 0;
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT * FROM CHUYENBAY WHERE MACH = @MACH";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MACH", maChuyen);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ChuyenBay = new ChuyenBay
                        {
                            MACH = reader["MACH"].ToString(),
                            CHUYEN = reader["CHUYEN"].ToString(),
                            DDI = reader["DDI"].ToString(),
                            DDEN = reader["DDEN"].ToString(),
                            NGAY = Convert.ToDateTime(reader["NGAY"]),
                            GBAY = TimeSpan.Parse(reader["GBAY"].ToString()),
                            GDEN = TimeSpan.Parse(reader["GDEN"].ToString()),
                            THUONG = Convert.ToInt32(reader["THUONG"]),
                            VIP = Convert.ToInt32(reader["VIP"]),
                            MAMB = reader["MAMB"].ToString()
                        };
                    }
                }
            }
            var query1 = @"SELECT HK.MAHK, HK.HOTEN, HK.DIENTHOAI, CT.LOAIGHE, CT.SOGHE
                         FROM HANHKHACH HK
                         JOIN CT_CB CT ON HK.MAHK = CT.MAHK
                         WHERE CT.MACH = @MACH";
            using (var command = new SqlCommand(query1, connection))
            {
                command.Parameters.AddWithValue("@MACH", maChuyen);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hanhKhachs.Add(new CT_CB_HK
                        {
                            MAHK = reader["MAHK"].ToString(),
                            HOTEN = reader["HOTEN"].ToString(),
                            DIENTHOAI = reader["DIENTHOAI"].ToString(),
                            LOAIGHE = Convert.ToBoolean(reader["LOAIGHE"]),
                            SOGHE = reader["SOGHE"].ToString()
                        });
                    }
                }
            }
            var query2 = "SELECT LOAIGHE, COUNT(*) as SoLuong FROM CT_CB WHERE MACH = @MACH GROUP BY LOAIGHE";
            using (var command = new SqlCommand(query2, connection))
            {
                command.Parameters.AddWithValue("@MACH", maChuyen);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bool loaiGhe = Convert.ToBoolean(reader["LOAIGHE"]);
                        int soLuong = Convert.ToInt32(reader["SoLuong"]);
                        if (loaiGhe) vip = soLuong;
                        else thuong = soLuong;
                    }
                }
            }
        }
        return new ChuyenBayViewModel
        {
            ChuyenBay = ChuyenBay,
            HanhKhachs = hanhKhachs,
            SoHanhKhachThuong = thuong,
            SoHanhKhachVIP = vip
        };

    }

    // Lấy danh sách hành khách của chuyến bay
    // public List<CT_CB_HK> GetHanhKhachByChuyenBay(string maChuyen)
    // {
    //     var hanhKhachs = new List<CT_CB_HK>();
    //     using (var connection = new SqlConnection(_connectionString))
    //     {
    //         connection.Open();
    //         var query = @"SELECT HK.MAHK, HK.HOTEN, HK.DIENTHOAI, CT.LOAIGHE, CT.SOGHE
    //                      FROM HANHKHACH HK
    //                      JOIN CT_CB CT ON HK.MAHK = CT.MAHK
    //                      WHERE CT.MACH = @MACH";
    //         using (var command = new SqlCommand(query, connection))
    //         {
    //             command.Parameters.AddWithValue("@MACH", maChuyen);
    //             using (var reader = command.ExecuteReader())
    //             {
    //                 while (reader.Read())
    //                 {
    //                     hanhKhachs.Add(new CT_CB_HK
    //                     {
    //                         MAHK = reader["MAHK"].ToString(),
    //                         HOTEN = reader["HOTEN"].ToString(),
    //                         DIENTHOAI = reader["DIENTHOAI"].ToString(),
    //                         LOAIGHE = Convert.ToBoolean(reader["LOAIGHE"]),
    //                         SOGHE = reader["SOGHE"].ToString()
    //                     });
    //                 }
    //             }
    //         }
    //     }
    //     return hanhKhachs;
    // }

    // // Đếm số hành khách Thường và VIP
    // public (int Thuong, int VIP) GetSoHanhKhachLoaiGhe(string maChuyen)
    // {
    //     int thuong = 0, vip = 0;
    //     using (var connection = new SqlConnection(_connectionString))
    //     {
    //         connection.Open();
    //         var query = "SELECT LOAIGHE, COUNT(*) as SoLuong FROM CT_CB WHERE MACH = @MACH GROUP BY LOAIGHE";
    //         using (var command = new SqlCommand(query, connection))
    //         {
    //             command.Parameters.AddWithValue("@MACH", maChuyen);
    //             using (var reader = command.ExecuteReader())
    //             {
    //                 while (reader.Read())
    //                 {
    //                     bool loaiGhe = Convert.ToBoolean(reader["LOAIGHE"]);
    //                     int soLuong = Convert.ToInt32(reader["SoLuong"]);
    //                     if (loaiGhe) vip = soLuong;
    //                     else thuong = soLuong;
    //                 }
    //             }
    //         }
    //     }
    //     return (thuong, vip);
    // }

    public void DeleteHK(string mahk, string mach)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string sql = "delete from CT_CB where MAHK = @MAHK and MACH = @MACH";
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@MAHK", mahk);
                cmd.Parameters.AddWithValue("@MACH", mach);
                cmd.ExecuteNonQuery();
            }
        }

    }

    public void UpdateHK(string MAHK, string MACH, string SOGHE, bool LOAIGHE)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string sql = "Update CT_CB set LOAIGHE=@LOAIGHE, SOGHE=@SOGHE where MAHK=@MAHK and MACH=@MACH";
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@MACH", MACH);
                cmd.Parameters.AddWithValue("@MAHK", MAHK);
                cmd.Parameters.AddWithValue("@LOAIGHE", LOAIGHE);
                cmd.Parameters.AddWithValue("@SOGHE", SOGHE);
                cmd.ExecuteNonQuery();
            }
        }

    }

    public void ThemHK(string MAHK, string MACH, string SOGHE, bool LOAIGHE)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string sql = "Insert into CT_CB(MACH,MAHK,SOGHE,LOAIGHE) values(@MACH,@MAHK,@SOGHE,@LOAIGHE)";
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@MACH", MACH);
                cmd.Parameters.AddWithValue("@MAHK", MAHK);
                cmd.Parameters.AddWithValue("@SOGHE", SOGHE);
                cmd.Parameters.AddWithValue("@LOAIGHE", LOAIGHE);

                cmd.ExecuteNonQuery();
            }
        }

    }
}