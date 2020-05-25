Vue.component('score-table', {
  data() {
    return {
      userScore: [],
      examParts: [],
      totalNum: 1,
      currentPage: 1,
      isLoading: false,
      searchKey: '',
      selectvalue: null,
      isShowPaper: false,
      title: '',
      fills: [],
      fillAnsNum: [],
      judgments: [],
      multiples: [],
      singles: [],
      singleSort: [],
      multipleSort: [],
      judgmentSort: [],
      answers: null,
      isPaperLoading: false,
      fillAnswers: []
    }
  },
  template: `
       <div>
        <div class="main-header">考生成绩表</div>
        <div class="search-wrap">
          <div>
            <el-input :value="searchKey" placeholder="请输入姓名" @input="inputChange"></el-input>
            <el-button type="primary" @click="search">搜索</el-button>
          </div>
          <div>
            <el-select v-model="selectvalue" placeholder="请选择显示日期" @change="selectChange" class="score-select" clearable>
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
        <el-table :data="userScore" v-loading="isLoading">
          <el-table-column prop="logId" label="ID"></el-table-column>
          <el-table-column prop="userName" label="姓名"></el-table-column>
          <el-table-column prop="date" label="考试日期"></el-table-column>
          <el-table-column prop="title" label="场次"></el-table-column>
          <el-table-column prop="examTitle" label="试卷名"></el-table-column>
          <el-table-column prop="score" label="分数"></el-table-column>
          <el-table-column prop="isSubmit" label="是否提交"></el-table-column>
          <el-table-column label="操作" align="center" prop="id">
            <template slot-scope="scope">
              <el-link type="success" @click="show(scope.row)">查看</el-link>
              <el-link type="danger" @click="remove(scope.row.logId)">删除</el-link>
            </template>
          </el-table-column>
        </el-table>
        <div class="pagination clearfix">
          <el-pagination background layout="prev, pager, next" :page-size="10" :total="totalNum" @current-change="onChangePage">
          </el-pagination>
        </div>
        <el-dialog
          title="试卷详情"
          :visible="isShowPaper"
          class="paper-more"
          @close="isShowPaper = false"
          center>
          <div v-loading="isPaperLoading">
            <div class="single-wrap">
              <p class="single-title big-title">一、选择题</p>
              <ul>
                <li class="single-item" v-for="item,index in singles" :key="item.EsId">
                  <span class="title-index">{{index + 1}}.</span>
                  <div v-html="item.SQTitle" class="title-content"></div>
                  <el-radio-group v-model="item.ans[0]" disabled>
                    <el-radio label="SQAns1">A. {{item.SQAns1}}</el-radio>
                    <el-radio label="SQAns2">B. {{item.SQAns2}}</el-radio>
                    <el-radio label="SQAns3">C. {{item.SQAns3}}</el-radio>
                    <el-radio label="SQTrueAns" style="color:#67C23A;">D. {{item.SQTrueAns}}</el-radio>
                  </el-radio-group>
                  <div class="answers">正确答案：{{item.SQTrueAns}}</div>
                </li>
              </ul>
            </div>
            <div class="multiple-wrap">
              <p class="multiple-title big-title">二、多选题</p>
              <ul>
                <li class="multiple-item" v-for="item,index in multiples" :key="item.EsId">
                  <span class="title-index">{{index + 1}}.</span>
                  <div v-html="item.multiple.MQTitle" class="title-content"></div>
                  <el-checkbox-group v-model="item.multiple.ans" disabled>
                    <el-checkbox v-if="item.multiple['MQAns1'] != null" label="MQAns1">{{item.multiple['MQAns1']}}</el-checkbox>
                    <el-checkbox v-if="item.multiple['MQAns2'] != null" label="MQAns2">{{item.multiple['MQAns2']}}</el-checkbox>
                    <el-checkbox v-if="item.multiple['MQAns3'] != null" label="MQAns3">{{item.multiple['MQAns3']}}</el-checkbox>
                    <el-checkbox v-if="item.multiple['MQAns4'] != null" label="MQAns4">{{item.multiple['MQAns4']}}</el-checkbox>
                    <el-checkbox v-if="item.multiple['MQAns5'] != null" label="MQAns5">{{item.multiple['MQAns5']}}</el-checkbox>
                    <el-checkbox v-if="item.multiple['MQAns6'] != null" label="MQAns6">{{item.multiple['MQAns6']}}</el-checkbox>
                    <el-checkbox v-if="item.multiple['MQAns7'] != null" label="MQAns7">{{item.multiple['MQAns7']}}</el-checkbox>
                  </el-checkbox-group>
                  <div class="answers-wrap">
                    正确答案：
                    <div v-for="ans,idx in item.mulAns">
                      <div class="answers">{{ item.multiple[ans.MAContent] }}</div>
                    </div>
                  </div>
                </li>
              </ul>
            </div>
            <div class="judgment-wrap">
              <p class="judgment-title big-title">三、判断题</p>
              <ul>
                <li class="judgment-item" v-for="item,index in judgments" :key="item.EsId">
                  <span class="title-index">{{index + 1}}.</span>
                  <div v-html="item.JQTitle" class="title-content"></div>
                  <el-radio-group v-model="item.ans[0]" disabled>
                    <el-radio label="JQFalseAns">{{item['JQFalseAns']}}</el-radio>
                    <el-radio label="JQTrueAns">{{item['JQTrueAns']}}</el-radio>
                  </el-radio-group>
                  <div class="answers">正确答案：{{item.JQTrueAns}}</div>
                </li>
              </ul>
            </div>
            <div class="fill-wrap">
              <p class="fill-title big-title">四、填空题</p>
              <ul>
                <li class="fill-item" v-for="item,index in fills" :key="item.EsId">
                  <span class="title-index">{{index + 1}}.</span>
                  <div v-html="item.FQTitle" class="title-content"></div>
                  <div class="fill-answer">
                    <p v-for="num,numIdx in fillAnsNum[index]" :key="numIdx" class="fill-answer-item">
                      <span>第{{numIdx + 1}}个空：</span>
                      <el-input v-model="item.ans[numIdx]" placeholder="请输入内容" disabled></el-input>
                    </p>
                  </div>
                  <div class="answers-wrap" v-for="ans,idx in fillAnswers">
                    <div class="answer-wrap" v-if="ans[0].FQId === item.FQId">
                      正确答案：
                      <div class="answers" v-for="a in ans">{{a.FAContent}}</div>
                    </div>
                  </div>
                </li>
              </ul>
            </div>
          </div>
          <span slot="footer" class="dialog-footer">
            <el-button type="primary" @click="isShowPaper = false">确 定</el-button>
          </span>
        </el-dialog>
      </div>
    `,
  methods: {
    onChangePage: function (val) {
      this.currentPage = val;
    },
    remove: function (id) {
      this.$confirm('此操作将删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        axios.post('/UserScore/Remove', { id }).then(res => {
          const { data } = res;
          if (data.code == 0) {
            this.$message({
              type: 'success',
              message: data.message
            });
            this.getUserScore('', this.selectvalue);
          } else {
            this.$message.error(data.message);
          }
        });
      });
    },
    inputChange: function (val) {
      this.searchKey = val;
    },
    search: function () {
      this.getUserScore(this.searchKey, this.selectvalue);
    },
    show: function (row) {
      if (row.isSubmit === '否') {
        return this.$confirm('该考生没有提交试卷', '提示', {
          type: 'error'
        }).then(() => { }).catch(() => { });
      }
      this.isShowPaper = true;

      this.getPaper(row.userId)
      console.log(row)
    },
    exportScore: function () {
      console.log('导出成绩单');
    },
    selectChange: function (val) {
      this.selectvalue = val;
    },
    // 获取学生成绩数据
    getUserScore: function (keyword, ptId) {
      this.isLoading = true;
      axios.get('/UserScore/GetUserScore', {
        params: {
          pageIndex: this.currentPage,
          keyword,
          ptId
        }
      }).then(res => {
        this.isLoading = false;
        var data = res.data;

        var count = data.slice(data.length - 1);
        var logs = data.slice(0, data.length - 1);

        this.totalNum = count[0];
        this.userScore = logs;
      });
    },
    // 获取试卷详情
    getPaper(userId) {
      this.isPaperLoading = true;
      axios.post("/StartExam/GetExamPaperIsSubmit", {
        id: userId,
        isSubmit: true
      }).then(res => {
        this.isPaperLoading = false;
        var data = res.data;

        if (data.code === 1) {
          this.$confirm('获取试卷失败，请检查网络或服务器', '提示', {
            confirmButtonText: '确定',
            type: 'error'
          }).then(() => {
          }).catch(() => {
          });
        }

        this.title = data.title;

        // 添加单选题答案存放数组
        for (var i = 0; i < data.singles.length; i++) {
          data.singles[i].ans = [];
          data.singles[i].ans.push('');
        }
        this.singles = data.singles;

        // 添加多选题答案存放数组
        for (var i = 0; i < data.multiples.length; i++) {
          data.multiples[i].multiple.ans = [];
        }
        this.multiples = data.multiples;

        // 添加判断题答案存放数组
        for (var i = 0; i < data.judgments.length; i++) {
          data.judgments[i].ans = [];
          data.judgments[i].ans.push('');
        }
        this.judgments = data.judgments;

        // 添加填空题答案存放数组
        for (var i = 0; i < data.fills.length; i++) {
          data.fills[i].ans = [];
          for (var j = 0; j < data.fillAnsNum[i]; j++) {
            data.fills[i].ans[j] = '';
          }
        }

        this.fills = data.fills;
        this.fillAnsNum = data.fillAnsNum;
        this.fillAnswers = data.fillAnswers;

        if (data.answers != null) {
          this.answers = data.answers;
          this.loadAnswer(this.answers);
        }
      });
    },
    // 加载答案
    loadAnswer(ansStr) {
      var ansObj = JSON.parse(ansStr);

      var singleLength = this.singles.length;
      var multipleLength = this.multiples.length;
      var judgmentLength = this.judgments.length;
      var fillLength = this.fills.length;

      for (var i = 0; i < ansObj.length; i++) {
        for (var j = 0; j < singleLength; j++) {
          if (this.singles[j].EsId === ansObj[i].EsId) {
            this.singles[j].ans = ansObj[i].ans;
          }
        }
        for (var j = 0; j < multipleLength; j++) {
          if (this.multiples[j].multiple.EsId === ansObj[i].EsId) {
            this.multiples[j].multiple.ans = ansObj[i].ans;
          }
        }
        for (var j = 0; j < judgmentLength; j++) {
          if (this.judgments[j].EsId === ansObj[i].EsId) {
            this.judgments[j].ans = ansObj[i].ans;
          }
        }
        for (var j = 0; j < fillLength; j++) {
          if (this.fills[j].EsId === ansObj[i].EsId) {
            this.fills[j].ans = ansObj[i].ans;
          }
        }
      }
    }
  },
  watch: {
    currentPage: function () {
      this.getUserScore(this.searchKey, this.selectvalue);
    },
    selectvalue: function () {
      this.getUserScore(this.searchKey, this.selectvalue);
    }
  },
  created() {
    axios.get('/ExamPart/GetExamPart', {
      params: {
        pageIndex: 1,
        isPaging: false
      }
    }).then(res => {
      var data = res.data;

      var count = data.slice(data.length - 1);
      var exams = data.slice(0, data.length - 1);

      if (this.examParts.length < count[0]) {
        this.examParts.push(...exams);
      }
    });
    this.getUserScore('', this.selectvalue);
  }
});