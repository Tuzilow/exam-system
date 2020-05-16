Vue.component('change-user-exam', {
  props: {
    searchkey: {
      type: String,
      default: '',
    }
  },
  data() {
    return {
      users: [],
      totalNum: 1,
      parts: [],
      currentPage: 1,
      isLoading: true,
      isPartLoading: true
    }
  },
  template: `
       <div>
        <div>
          <div class="main-header">调整考生场次</div>
          <div class="search-wrap">
            <el-input :value="searchkey" placeholder="请输入姓名" @input="inputChange"></el-input>
            <el-button type="primary" @click="doSearch">搜索</el-button>
          </div>
          <el-table :data="users" v-loading="isLoading">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="account" label="账号"></el-table-column>
            <el-table-column prop="name" label="姓名"></el-table-column>
            <el-table-column prop="isJoin" label="是否进入考试" align="center"></el-table-column>
            <el-table-column label="场次" prop="partId" align="center">
              <template slot-scope="scope">
                <el-select :value="scope.row.partId" placeholder="请选择" @change="changePart($event, scope.row)">
                  <el-option
                    v-for="item in parts"
                    :key="item.id"
                    :label="item.date + '@' + item.title"
                    :value="item.id">
                  </el-option>
                </el-select>
              </template>
            </el-table-column>
          </el-table>
          <div class="pagination clearfix">
            <el-pagination background layout="prev, pager, next" :page-size="10" :total="totalNum" @current-change="onChangePage">
            </el-pagination>
          </div>
        </div>
      </div>
    `,
  methods: {
    onChangePage: function (val) {
      this.currentPage = val;
      this.getUsers();
    },
    inputChange: function (val) {
      this.$emit('input-change', val);
    },
    // 获取考生
    getUsers: function (pageIndex) {
      this.isLoading = true;
      axios.get('/ExamPart/GetUsers', {
        params: {
          pageIndex: pageIndex || this.currentPage,
          keyword: this.searchkey
        }
      }).then(res => {
        const { userInfo, totalCount } = res.data;

        this.users = JSON.parse(userInfo);
        this.totalNum = totalCount;

        // 判断场次是否获取完成
        this.isLoading = this.isPartLoading === true;
        if (this.isLoading === true) {
          let loadTimer = setInterval(() => {
            this.isLoading = this.isPartLoading === true;
            if (this.isLoading === false) {
              clearInterval(loadTimer);
            }
          }, 500);
        }
      });
    },
    // 获取场次
    getExamPart: function () {
      this.isPartLoading = true;
      axios.get('/ExamPart/GetExamPart', {
        params: {
          pageIndex: 1,
          isPaging: false
        }
      }).then(res => {
        this.isPartLoading = false;
        let { data } = res;
        let count = data.slice(data.length - 1);
        let exams = data.slice(0, data.length - 1);

        if (this.parts.length < count[0]) {
          this.parts.push(...exams);
        }
      });
    },
    // 改变场次
    changePart: function (val, info) {
      this.isLoading = true;
      this.users.forEach((user) => {
        if (user.id === info.id) {
          user.partId = val;
          axios.post('/ExamPart/ChangeUserPart', {
            userId: info.id,
            partId: val
          }).then(res => {
            this.isLoading = false;
            const { data } = res;
            if (data.code == 0) {
              this.$message({
                type: 'success',
                message: data.message
              });
            } else {
              this.$message.error(data.message);
            }
          });
        }
      });
    },
    doSearch: function () {
      this.getUsers(1);
    }
  },
  created() {
    this.getExamPart();
    this.getUsers();
  }
});