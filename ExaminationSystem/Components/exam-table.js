Vue.component('exam-table', {
  props: {
  },
  data() {
    return {
      exams: [],
      totalNum: 1,
      currentPage: 1,
      users: [],
      isShowStudent: false,
      isLoading: true,
      userLoading: true
    }
  },
  template: `
       <div>
        <div>
          <div class="main-header">场次列表</div>
          <div class="search-wrap">
            <div>
              数据按照日期倒序排列
            </div>
          </div>
          <el-table :data="exams" v-loading="isLoading">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="date" label="日期"></el-table-column>
            <el-table-column prop="start" label="开始时间"></el-table-column>
            <el-table-column prop="end" label="结束时间"></el-table-column>
            <el-table-column label="操作" align="center" prop="id">
              <template slot-scope="scope">
                <el-link type="success" @click="showAllStu(scope.row.id)">查看考生</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <div class="pagination clearfix">
            <el-pagination background layout="prev, pager, next" :page-size="10" :total="totalNum" @current-change="onChangePage">
            </el-pagination>
          </div>
        </div>
        <el-dialog title="考生一览" :visible="isShowStudent" @close="isShowStudent = false">
          <el-table :data="users" v-loading="userLoading">
            <el-table-column prop="userId" label="ID"></el-table-column>
            <el-table-column prop="account" label="账号"></el-table-column>
            <el-table-column prop="name" label="姓名"></el-table-column>
          </el-table>
        </el-dialog>
      </div>
    `,
  methods: {
    // 获取场次
    getExamParts: function () {
      this.isLoading = true;
      axios.get('/ExamPart/GetExamPart', { params: { pageIndex: this.currentPage } }).then(res => {
        let { data } = res;
        let count = data.slice(data.length - 1);

        this.exams = data.slice(0, data.length - 1);
        this.totalNum = count[0];
        this.isLoading = false;
      });
    },
    onChangePage: function (val) {
      this.currentPage = val;
    },
    // 展示该场次所有学生
    showAllStu: function (id) {
      this.userLoading = true;
      axios.get('/ExamPart/GetUserByExamPart', { params: { id } }).then(res => {
        this.users = res.data;
      });

      this.isShowStudent = true;
      this.userLoading = false;
    },
    // 删除
    remove: function (id) {
      this.$confirm('此操作将删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        axios.post('/ExamPart/Remove', { id }).then(res => {
          const { data } = res;
          if (data.code == 0) {
            this.$message({
              type: 'success',
              message: data.message
            });
            this.getExamParts(this.currentPage, '');
          } else {
            this.$message.error(data.message);
          }
        });
      });
    }
  },
  watch: {
    currentPage: function () {
      this.getExamParts();
    }
  },
  created() {
    this.getExamParts();
  }
});