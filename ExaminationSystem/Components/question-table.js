Vue.component('question-table', {
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
      singles: [{
        id: 1,
        title: '单选题1',
        sel1: 's1',
        sel2: 's2',
        sel3: 's3',
        trueSel: 't1', // 正确答案
        score: 2
      }],
      // 多选题
      multiples: [{
        id: 1,
        title: '多选题1',
        MQAns1: '吗',
        MQAns2: 'a',
        MQAns3: 'aa',
        MQAns4: 'ss',
        MQAns5: 'a',
        MQAns6: '???',
        MQAns7: 'a',
        trueSels: '', // 正确选项
        score: 2
      }],
      // 判断题
      judgments: [{
        id: 1,
        title: '判断题',
        trueSel: 'vvvv', // 正确答案
        falseSel: 'xxxx', // 错误答案
        score: 2
      }],
      // 填空题
      fills: [{
        id: 1,
        title: '<p>adfasdfsa()adfasdfsa</p>',
        answers: '', // 答案array
        score: 2
      }],
      totalNum: 1,
      currentPage: 1,
      isLoading: false
    }
  },
  template: `
       <div>
        <div class="main-header">题目列表</div>
        <div class="search-wrap">
          <el-select v-model="currentType" placeholder="请选择题目类型" class="score-select">
            <el-option
              v-for="item in questionType"
              :key="item.id"
              :label="item.type"
              :value="item.type">
            </el-option>
          </el-select>
        </div>
        <div v-loading="isLoading" class="question-table">
          <el-table :data="singles" v-if="currentType === '单选题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目" width="300"></el-table-column>
            <el-table-column prop="trueSel" label="正确答案"></el-table-column>
            <el-table-column prop="sel1" label="错误答案"></el-table-column>
            <el-table-column prop="sel2" label="错误答案"></el-table-column>
            <el-table-column prop="sel3" label="错误答案"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope" v-if="currentType === '单选题'">
                <el-link type="success" @click="show(scope.row)">查看</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="multiples" v-if="currentType === '多选题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目" width="300"></el-table-column>
            <el-table-column prop="MQAns1" label="选项1"></el-table-column>
            <el-table-column prop="MQAns2" label="选项2"></el-table-column>
            <el-table-column prop="MQAns3" label="选项3"></el-table-column>
            <el-table-column prop="MQAns4" label="选项4"></el-table-column>
            <el-table-column prop="MQAns5" label="选项5"></el-table-column>
            <el-table-column prop="MQAns6" label="选项6"></el-table-column>
            <el-table-column prop="MQAns7" label="选项7"></el-table-column>
            <el-table-column prop="trueSels" label="正确选项"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope" v-if="currentType === '多选题'">
                <el-link type="success" @click="show(scope.row)">查看</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="judgments" v-if="currentType === '判断题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目" width="300"></el-table-column>
            <el-table-column prop="trueSel" label="正确答案"></el-table-column>
            <el-table-column prop="falseSel" label="错误答案"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
            <el-table-column label="操作" prop="id" align="center">
              <template slot-scope="scope" v-if="currentType === '判断题'">
                <el-link type="success" @click="show(scope.row)">查看</el-link>
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="fills" v-if="currentType === '填空题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目" width="300"></el-table-column>
            <el-table-column prop="answers" label="答案"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
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
      console.log(row)

      let htmlStr = '<ul class="alert-warp">';
      for (var index in row) {
        htmlStr += '<li><span class="title">' +
          this.showTitle(index)
          + '</span><span class="content">' + (row[index] == null ? '无' : row[index]) + '</span></li>';
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
      }
      return res;
    },
    remove: function () {

    },
    onChangePage: function (val) {
      this.currentPage = val;
      switch (this.currentType) {
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
        let count = data.slice(data.length - 1);
        let singles = data.slice(0, data.length - 1);

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
    }
  },
  watch: {
    currentType: function (val, oldVal) {
      switch (val) {
        case '单选题': this.getSingle(); break;
        case '多选题': this.getMultiple(); break;
        case '判断题': this.getJudgment(); break;
        case '填空题': this.getFill(); break;
      }
    }
  },
  created() {
    this.getSingle(); // TODO 限制答案显示长度
  }
});