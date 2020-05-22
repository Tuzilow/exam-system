Vue.component('paper-table', {
  props: {
  },
  data() {
    return {
      isLoading: false,
      papers: [],
      parts: [],
      currentPage: 1,
      totalNum: 0,
      currentSelectPart: null
    }
  },
  template: `
       <div class="paper-table">
        <div class="main-header">试卷列表</div>
        <div class="search-wrap">
          <el-select clearable v-model="currentSelectPart" placeholder="请选择显示场次" @change="selectChange" @clear="clearSelect">
            <el-option
              v-for="item in parts"
              :key="item.id"
              :label="item.date+'@'+item.title"
              :value="item.id">
            </el-option>
          </el-select>
        </div>
        <div v-loading="isLoading">
          <el-table :data="papers">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="试卷名"></el-table-column>
            <el-table-column prop="date" label="日期"></el-table-column>
            <el-table-column prop="partTitle" label="场次"></el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope">
                <el-link type="success" @click="show(scope.row)">查看</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
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
    // 改变查看场次
    selectChange: function () {
      this.getPaper(this.currentSelectPart);
    },
    // 查看全部场次
    clearSelect: function () {
      this.getPaper();
    },
    // 改变页码
    onChangePage: function (val) {
      this.currentPage = val;
    },
    // 显示详情
    show: function (row) {
      let htmlStr = '<ul class="paper-alert-warp alert-warp">';
      for (var index in row) {
        htmlStr += '<li><span class="title">' +
          this.showTitle(index)
          + '</span><span class="content">' + (row[index] == null || row[index].length === 0 ? '无' : row[index]) + '</span></li>';
      }
      htmlStr += '</ul>'

      this.$alert(htmlStr, '试卷详情', {
        dangerouslyUseHTMLString: true,
        showConfirmButton: false
      }).catch(function (action) {
        console.log(action);
      });
    },
    // 格式化标题
    showTitle: function (indexName) {
      var res = '';
      switch (indexName) {
        case 'id': res = '试卷ID'; break;
        case 'title': res = '试卷名'; break;
        case 'singleNum': res = '单选题数量'; break;
        case 'singleScore': res = '单选题一题分数'; break;
        case 'multipleNum': res = '多选题数量'; break;
        case 'multipleScore': res = '多选题一题分数'; break;
        case 'judgmentNum': res = '判断题数量'; break;
        case 'judgmentScore': res = '判断题一题分数'; break;
        case 'fillNum': res = '填空题数量'; break;
        case 'fillScore': res = '填空题一题分数'; break;
        case 'partId': res = '场次ID'; break;
        case 'date': res = '日期'; break;
        case 'partTitle': res = '时间'; break;
        case 'tags': res = '标签'; break;
      }
      return res;
    },
    // 移除试卷
    remove: function (id) {
      this.$confirm('此操作将删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        axios.post('/Paper/RemovePaper', { id }).then(res => {
          this.isLoading = false;
          const { data } = res;
          if (data.code === 1) {
            return this.$message.error(data.message);
          }

          this.getPaper();

          return this.$message({
            message: data.message,
            type: 'success'
          });
        });
      }).catch(err => {
        console.log(err);
      });
    },
    // 获取场次
    getExamPart: function () {
      axios.get('/ExamPart/GetExamPart', {
        params: {
          pageIndex: 1,
          isPaging: false
        }
      }).then(res => {
        let { data } = res;
        let exams = data.slice(0, data.length - 1);

        this.parts = exams;
      });
    },
    // 获取试卷
    getPaper: function (partId = 0) {
      this.isLoading = true;
      axios.get('/Paper/GetPaper', {
        params: {
          pageIndex: this.currentPage,
          selectPartId: partId
        }
      }).then(res => {
        this.isLoading = false;
        let { data } = res;
        console.log(partId, data)
        let papers = data.slice(0, data.length - 1);

        this.papers = papers;
      });
    }
  },
  created() {
    this.getExamPart();
    this.getPaper();
  }
});