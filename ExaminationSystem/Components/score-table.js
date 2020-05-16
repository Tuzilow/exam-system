Vue.component('score-table', {
  props: {
    searchkey: {
      type: String,
      default: '',
    },
    selectvalue: {
      type: String,
      default: '',
    }
  },
  data() {
    return {
      userScore: [],
      examParts: [],
      totalNumToScore: 1,
      currentPage: 1
    }
  },
  template: `
       <div>
        <div class="main-header">考生成绩表</div>
        <div class="search-wrap">
          <div>
            <el-input :value="searchkey" placeholder="请输入姓名" @input="inputChange"></el-input>
            <el-button type="primary" @click="search">搜索</el-button>
          </div>
          <div>
            <el-select v-model="selectvalue" placeholder="请选择显示日期" @change="selectChange" class="score-select">
              <el-option
                v-for="item in examParts"
                :key="item.id"
                :label="item.date+'@'+item.title"
                :value="item.id">
              </el-option>
            </el-select>
            <el-button type="primary" @click="exportScore" style="margin-right: 1rem;">导出</el-button>
          </div>
        </div>
        <el-table :data="userScore">
          <el-table-column prop="id" label="ID"></el-table-column>
          <el-table-column prop="name" label="姓名"></el-table-column>
          <el-table-column prop="date" label="考试日期"></el-table-column>
          <el-table-column prop="exam" label="场次"></el-table-column>
          <el-table-column prop="score" label="分数"></el-table-column>
          <el-table-column label="操作" align="center" prop="id">
            <template slot-scope="scope">
              <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
            </template>
          </el-table-column>
        </el-table>
        <div class="pagination clearfix">
          <el-pagination background layout="prev, pager, next" :page-size="10" :total="totalNumToScore" @current-change="onChangePage">
          </el-pagination>
        </div>
      </div>
    `,
  methods: {
    onChangePage: function (val) {
      this.currentPage = val;
    },
    inputChange: function (val) {
      this.$emit('input-change', val);
    },
    search: function () {
      console.log(this.searchkey)
    },
    remove: function (id) {
      console.log('删除', id)
    },
    exportScore: function () {
      console.log('导出成绩单');
    },
    selectChange: function (val) {
      this.$emit('select-change', val);
    },
    // 获取学生成绩数据
    getUserScore: function (pageIndex = 1) {
      axios.get('/UserScore/GetUserScoreByPartId', {
        params: {
          id: this.selectvalue,
          pageIndex
        }
      }).then(res => {
        this.userScore = res.data;
      });
    }
  },
  watch: {
    currentPage: function (val, oldVal) {
      this.getUserScore(val);
    }
  },
  created() {
    axios.get('/UserScore/GetExamPart').then(res => {
      this.examParts = res.data;
    });
    this.getUserScore();
  }
});