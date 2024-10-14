using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace De01
{
    public partial class frmSinhvien : Form
    {
        private string connectionString = "your_connection_string_here";
        public frmSinhvien()
        {
            InitializeComponent();
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
        }
        private void FrmSinhvien_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureDataGridView();

                // Load student data into DataGridView
                LoadSinhVienData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadSinhVienData()
        {
            using (var context = new StudentModel())
            {
                // Lấy danh sách sinh viên từ cơ sở dữ liệu với thông tin khoa
                List<Sinhvien> listSinhviens = context.Sinhviens
                    .Include(s => s.Lop) // Sử dụng Include để lấy thông tin Faculty
                    .ToList();

                // Gán dữ liệu vào DataGridView
                BindGrid(listSinhviens);
            }
        }
        private void LoadLopData()
        {
            try
            {
                using (var context = new StudentModel())
                {
                    // Lấy danh sách lớp học từ cơ sở dữ liệu
                    var lopList = context.Lops.ToList();

                    // Dọn sạch ComboBox trước khi thêm
                    cboLop.Items.Clear();

                    // Thêm lớp học vào ComboBox
                    foreach (var lop in lopList)
                    {
                        cboLop.Items.Add(lop); // Giả sử lớp có phương thức ToString() trả về tên lớp
                    }

                    // Tùy chọn chọn lớp đầu tiên
                    if (cboLop.Items.Count > 0)
                    {
                        cboLop.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindGrid(List<Sinhvien> listSinhviens)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = null; // Xóa nguồn dữ liệu hiện tại
            dataGridView1.DataSource = listSinhviens; // Gán danh sách sinh viên vào DataGridView
        }
        private void ConfigureDataGridView()
        {
            // Đặt tên cho DataGridView
            dataGridView1.Name = "dataGridView1";

            // Thiết lập chế độ tự động điều chỉnh kích thước cột
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Xóa tất cả các cột trước khi thêm cột mới (nếu cần thiết)
            dataGridView1.Columns.Clear();

            // Tạo và cấu hình cột "Mã SV"
            var colMaSV = new DataGridViewTextBoxColumn
            {
                HeaderText = "Mã SV",
                MinimumWidth = 100,
                Name = "colMaSV",
                DataPropertyName = "MaSV" // Liên kết với thuộc tính MaSV trong lớp Sinhvien
            };
            dataGridView1.Columns.Add(colMaSV);

            // Tạo và cấu hình cột "Họ và tên"
            var colHotenSV = new DataGridViewTextBoxColumn
            {
                HeaderText = "Họ và tên",
                MinimumWidth = 150,
                Name = "colHotenSV",
                DataPropertyName = "HotenSV" // Liên kết với thuộc tính HotenSV trong lớp Sinhvien
            };
            dataGridView1.Columns.Add(colHotenSV);

            // Tạo và cấu hình cột "Ngày sinh"
            var colNgaysinh = new DataGridViewTextBoxColumn
            {
                HeaderText = "Ngày sinh",
                MinimumWidth = 100,
                Name = "colNgaysinh",
                DataPropertyName = "NgaySinh", // Liên kết với thuộc tính NgaySinh trong lớp Sinhvien
                DefaultCellStyle = { Format = "dd/MM/yyyy" } // Định dạng ngày
            };
            dataGridView1.Columns.Add(colNgaysinh);

            // Tạo và cấu hình cột "Lớp"
            var colLop = new DataGridViewTextBoxColumn
            {
                HeaderText = "Lớp",
                MinimumWidth = 100,
                Name = "colLop",
                DataPropertyName = "Lop" // Liên kết với thuộc tính Lop trong lớp Sinhvien
            };
            dataGridView1.Columns.Add(colLop);

            // Thêm sự kiện khi người dùng nhấn vào một hàng trong DataGridView
            dataGridView1.CellClick += dataGridView1_CellClick;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng có nhấn vào một hàng hợp lệ không
            if (e.RowIndex >= 0)
            {
                var selectedSinhvien = dataGridView1.Rows[e.RowIndex].DataBoundItem as Sinhvien;

                if (selectedSinhvien != null)
                {
                    // Hiển thị thông tin sinh viên trên các textbox
                    txtMaSV.Text = selectedSinhvien.MaSV;
                    txtHotenSV.Text = selectedSinhvien.HotenSV;

                    // Kiểm tra xem NgaySinh có giá trị null không
                    dtNgaysinh.Value = selectedSinhvien.NgaySinh ?? DateTime.Now; // Sử dụng ngày hiện tại nếu NgaySinh là null

                    // Chọn lớp trong ComboBox
                    cboLop.SelectedItem = selectedSinhvien.Lop;
                }
            }
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                // Gán giá trị của sinh viên vào các control nhập liệu
                txtMaSV.Text = dataGridView1.CurrentRow.Cells["MaSV"].Value.ToString();
                txtHotenSV.Text = dataGridView1.CurrentRow.Cells["HotenSV"].Value.ToString();
                dtNgaysinh.Value = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["Ngaysinh"].Value);
                cboLop.Text = dataGridView1.CurrentRow.Cells["Lop"].Value.ToString();
            }


        }

        private void btThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new StudentModel())
                {
                    Lop selectedLop = cboLop.SelectedItem as Lop;
                    // Tạo đối tượng sinh viên mới
                    Sinhvien newSinhvien = new Sinhvien
                    {
                        MaSV = txtMaSV.Text,
                        HotenSV = txtHotenSV.Text,
                        NgaySinh = dtNgaysinh.Value,
                        Lop = selectedLop
                    };

                    // Thêm sinh viên vào database
                    context.Sinhviens.Add(newSinhvien);
                    context.SaveChanges();

                    // Refresh DataGridView
                    LoadSinhVienData();
                    MessageBox.Show("Thêm sinh viên thành công!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new StudentModel())
                {
                    // Lấy mã sinh viên từ ô nhập
                    string maSV = txtMaSV.Text;

                    // Tìm sinh viên trong cơ sở dữ liệu
                    var existingSinhvien = context.Sinhviens.FirstOrDefault(sv => sv.MaSV == maSV);
                    if (existingSinhvien == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Cập nhật thông tin sinh viên
                    existingSinhvien.HotenSV = txtHotenSV.Text;
                    existingSinhvien.NgaySinh = dtNgaysinh.Value;

                    // Lấy lớp được chọn
                    Lop selectedLop = cboLop.SelectedItem as Lop;
                    if (selectedLop == null)
                    {
                        MessageBox.Show("Vui lòng chọn lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    existingSinhvien.Lop = selectedLop;

                    // Lưu thay đổi
                    context.SaveChanges();

                    // Refresh DataGridView
                    LoadSinhVienData();
                    MessageBox.Show("Sửa thông tin sinh viên thành công!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return; // Người dùng chọn không xóa
            }

            try
            {
                using (var context = new StudentModel())
                {
                    // Lấy mã sinh viên từ ô nhập
                    string maSV = txtMaSV.Text;

                    // Tìm sinh viên trong cơ sở dữ liệu
                    var sinhvienToDelete = context.Sinhviens.FirstOrDefault(sv => sv.MaSV == maSV);
                    if (sinhvienToDelete == null)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Xóa sinh viên
                    context.Sinhviens.Remove(sinhvienToDelete);
                    context.SaveChanges();

                    // Refresh DataGridView
                    LoadSinhVienData();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Thoát ứng dụng
            }
        }

        private void cboLop_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
