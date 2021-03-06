﻿Vue.component('question-table', {
  props: {
  },
  data() {
    return {
      questionType: [
        { id: 1, type: '单选题' },
        { id: 2, type: '多选题' },
        { id: 3, type: '判断题' },
        { id: 4, type: '填空题' },
      ],
      currentType: '单选题',
      // 单选题
      singles: [],
      // 多选题
      multiples: [],
      // 判断题
      judgments: [],
      // 填空题
      fills: [],
      totalNum: 1,
      currentPage: 1,
      isLoading: false,
      tags: [],
      selTag: null, // 选中标签
      loginRoleId: 0
    }
  },
  template: `
       <div>
        <div class="main-header">题目列表</div>
        <div class="search-wrap">
          按条件过滤题目
          <el-select v-model="currentType" placeholder="请选择题目类型" class="score-select">
            <el-option
              v-for="item in questionType"
              :key="item.id"
              :label="item.type"
              :value="item.type">
            </el-option>
          </el-select>
          <el-select v-model="selTag" filterable placeholder="请选择标签" class="select-tag" clearable @clear="getQuestions(currentType)">
            <el-option
              v-for="item in tags"
              :key="item.id"
              :label="item.name"
              :value="item.id"
              v-if="isShowTag(item)"
            >
            </el-option>
          </el-select>
        </div>
        <div v-loading="isLoading" class="question-table">
          <el-table :data="singles" v-if="currentType === '单选题'">
            <el-table-column prop="title" label="题目" width="300">
              <template slot-scope="scope" v-if="currentType === '单选题'">
                <p v-html="scope.row.title" class="title-detail"></p>
              </template>
            </el-table-column>
            <el-table-column prop="trueSel" label="正确答案"></el-table-column>
            <el-table-column prop="sel1" label="错误答案"></el-table-column>
            <el-table-column prop="sel2" label="错误答案"></el-table-column>
            <el-table-column prop="sel3" label="错误答案"></el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope" v-if="currentType === '单选题'">
                <el-link type="success" @click="show(scope.row)">查看</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="multiples" v-if="currentType === '多选题'">
            <el-table-column prop="title" label="题目" width="300">
              <template slot-scope="scope" v-if="currentType === '多选题'">
                <p v-html="scope.row.title" class="title-detail"></p>
              </template>
            </el-table-column>
            <el-table-column prop="MQAns1" label="选项1"></el-table-column>
            <el-table-column prop="MQAns2" label="选项2"></el-table-column>
            <el-table-column prop="MQAns3" label="选项3"></el-table-column>
            <el-table-column prop="MQAns4" label="选项4"></el-table-column>
            <el-table-column prop="MQAns5" label="选项5"></el-table-column>
            <el-table-column prop="MQAns6" label="选项6"></el-table-column>
            <el-table-column prop="MQAns7" label="选项7"></el-table-column>
            <el-table-column prop="trueSels" label="正确选项"></el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope" v-if="currentType === '多选题'">
                <el-link type="success" @click="show(scope.row)">查看</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="judgments" v-if="currentType === '判断题'">
            <el-table-column prop="title" label="题目" width="300">
              <template slot-scope="scope" v-if="currentType === '判断题'">
                <p v-html="scope.row.title" class="title-detail"></p>
              </template>
            </el-table-column>
            <el-table-column prop="isTrue" label="答案">
              <template slot-scope="scope" v-if="currentType === '判断题'">
                <span v-if="scope.row.isTrue === true">对</span>
                <span v-if="scope.row.isTrue === false">错</span>
              </template>
            </el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope" v-if="currentType === '判断题'">
                <el-link type="success" @click="show(scope.row)">查看</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="fills" v-if="currentType === '填空题'">
            <el-table-column prop="title" label="题目" width="300">
              <template slot-scope="scope" v-if="currentType === '填空题'">
                <p v-html="scope.row.title" class="title-detail"></p>
              </template>
            </el-table-column>
            <el-table-column prop="answers" label="答案"></el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope" v-if="currentType === '填空题'">
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
    // 题目详情
    show: function (row) {
      let htmlStr = '<ul class="alert-warp">';
      for (var index in row) {
        if (this.showTitle(index) == '分数') {
          continue;
        }
        htmlStr += '<li><span class="title">' +
          this.showTitle(index)
          + '</span><span class="content">' + (row[index] == null ? '无' : (row[index] == true ? '对' : (row[index] == false ? '错' : row[index]))) + '</span></li>';
      }
      htmlStr += '</ul>'

      this.$alert(htmlStr, '题目详情', {
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
        case 'id': res = 'ID'; break;
        case 'title': res = '标题'; break;
        case 'trueSel': res = '正确答案'; break;
        case 'falseSel': res = '错误答案'; break;
        case 'sel1': res = '选项1'; break;
        case 'sel2': res = '选项2'; break;
        case 'sel3': res = '选项3'; break;
        case 'score': res = '分数'; break;
        case 'MQAns1': res = '选项1'; break;
        case 'MQAns2': res = '选项2'; break;
        case 'MQAns3': res = '选项3'; break;
        case 'MQAns4': res = '选项4'; break;
        case 'MQAns5': res = '选项5'; break;
        case 'MQAns6': res = '选项6'; break;
        case 'MQAns7': res = '选项7'; break;
        case 'trueSels': res = '正确选项'; break;
        case 'answers': res = '答案'; break;
        case 'isTrue': res = '答案'; break;
      }
      return res;
    },
    // 删除题目
    remove: function (id) {
      this.$confirm('此操作将删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        axios.post('/Question/RemoveQuestion', { id }).then(res => {
          this.isLoading = false;
          const { data } = res;
          if (data.code === 1) {
            return this.$message.error(data.message);
          }

          this.getQuestions(this.currentType);

          return this.$message({
            message: data.message,
            type: 'success'
          });
        });
      }).catch(err => {
        console.log(err);
      });
    },
    onChangePage: function (val) {
      this.currentPage = val;
      this.getQuestions(this.currentType);
    },
    // 获取题目
    getQuestions: function (type) {
      switch (type) {
        case '单选题': this.getSingle(); break;
        case '多选题': this.getMultiple(); break;
        case '判断题': this.getJudgment(); break;
        case '填空题': this.getFill(); break;
      }
    },
    // 获取单选题
    getSingle: function () {
      this.isLoading = true;
      axios.get('/Question/GetSingle', {
        params: {
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        var count = data.slice(data.length - 1);
        var singles = data.slice(0, data.length - 1);

        this.totalNum = count[0];
        this.singles = singles;
      });
    },
    getSingleByTagId: function (tagId) {
      this.isLoading = true;
      axios.get('/Question/GetSingleByTagId', {
        params: {
          tagId,
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        var count = data.slice(data.length - 1);
        var singles = data.slice(0, data.length - 1);

        this.totalNum = count[0];
        this.singles = singles;
      });
    },
    // 获取多选题
    getMultiple: function () {
      this.isLoading = true;
      axios.get('/Question/GetMultiple', {
        params: {
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        let count = data.slice(data.length - 1);
        let multiples = data.slice(0, data.length - 1);
        // 将答案编号转为答案内容
        for (var mul in multiples) {
          var oldTrueSels = multiples[mul].trueSels;

          var nowTrueSels = [];
          for (var i = 0; i < oldTrueSels.length; i++) {
            nowTrueSels.push(multiples[mul][oldTrueSels[i]]);
          }

          multiples[mul].trueSels = nowTrueSels.join(',');
        }

        this.totalNum = count[0];
        this.multiples = multiples;
      });
    },
    getMultipleByTagId: function (tagId) {
      this.isLoading = true;
      axios.get('/Question/GetMultipleByTagId', {
        params: {
          tagId,
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        let count = data.slice(data.length - 1);
        let multiples = data.slice(0, data.length - 1);
        // 将答案编号转为答案内容
        for (var mul in multiples) {
          var oldTrueSels = multiples[mul].trueSels;

          var nowTrueSels = [];
          for (var i = 0; i < oldTrueSels.length; i++) {
            nowTrueSels.push(multiples[mul][oldTrueSels[i]]);
          }

          multiples[mul].trueSels = nowTrueSels.join(',');
        }

        this.totalNum = count[0];
        this.multiples = multiples;
      });
    },
    // 获取判断题
    getJudgment: function () {
      this.isLoading = true;
      axios.get('/Question/GetJudgment', {
        params: {
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        let count = data.slice(data.length - 1);
        let judgments = data.slice(0, data.length - 1);

        this.totalNum = count[0];
        this.judgments = judgments;
      });
    },
    getJudgmentByTagId: function (tagId) {
      this.isLoading = true;
      axios.get('/Question/GetJudgmentByTagId', {
        params: {
          tagId,
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        let count = data.slice(data.length - 1);
        let judgments = data.slice(0, data.length - 1);

        this.totalNum = count[0];
        this.judgments = judgments;
      });
    },
    // 获取填空题
    getFill: function () {
      this.isLoading = true;
      axios.get('/Question/GetFill', {
        params: {
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        let count = data.slice(data.length - 1);
        let fills = data.slice(0, data.length - 1);

        // 将答案编号转为答案内容
        for (var fillIndex in fills) {
          var oldAns = fills[fillIndex].answers;

          fills[fillIndex].answers = oldAns.join(',');
        }

        this.totalNum = count[0];
        this.fills = fills;
      });
    },
    getFillByTagId: function (tagId) {
      this.isLoading = true;
      axios.get('/Question/GetFillByTagId', {
        params: {
          tagId,
          pageIndex: this.currentPage
        }
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        let count = data.slice(data.length - 1);
        let fills = data.slice(0, data.length - 1);

        // 将答案编号转为答案内容
        for (var fillIndex in fills) {
          var oldAns = fills[fillIndex].answers;

          fills[fillIndex].answers = oldAns.join(',');
        }

        this.totalNum = count[0];
        this.fills = fills;
      });
    },
    // 获得全部标签
    getTags: function () {
      this.isLoading = true;
      axios.get('/Tag/GetTags').then(res => {
        this.isLoading = false;
        var data = res.data;
   
        this.tags = data;
        
      });
    },
    getRoleId: function () {
      this.loginRoleId = parseInt(localStorage.getItem('roleId'));
    },
    isShowTag: function (tag) {
      if (this.loginRoleId === 3) {
        return true;
      } else if (!tag.isPrivate) {
        return true;
      }
      return false;
    }
  },
  watch: {
    currentType: function (val, oldVal) {
      this.currentPage = 1;
      this.getQuestions(val);
    },
    selTag: function (val, oldVal) {
      this.currentPage = 1;
      if (val !== 0) {
        switch (this.currentType) {
          case '单选题': this.getSingleByTagId(val); break;
          case '多选题': this.getMultipleByTagId(val); break;
          case '判断题': this.getJudgmentByTagId(val); break;
          case '填空题': this.getFillByTagId(val); break;
        }
      } else {
        this.getQuestions(this.currentType);
      }
    }
  },
  created() {
    this.getSingle(); // TODO 限制答案显示长度
    this.getTags();
    this.getRoleId();
  }
});