Vue.component('new-paper', {
  props: {
  },
  data() {
    return {
      paper: {
        title: '',
        parts: [
          { id: 1, title: '' }
        ],
        single: { num: 10, score: 3, total: 30 },
        multiple: { num: 10, score: 2, total: 20 },
        judgment: { num: 10, score: 2, total: 20 },
        fill: { num: 10, score: 3, total: 30 },
      },
      tags: [],
      selTags: [],
      selPart: null,
      maxScore: 0,
      isLoading: false,
      tagPercent: [
      ]
    };
  },
  template: `
       <div class="new-paper">
        <div class="main-header">添加试卷</div>
        <div class="main-content">
          <el-form :model="paper" label-position="top" v-loading="isLoading"  class="new-paper-form">
            <el-form-item label="试卷名称" class="new-title">
              <el-input v-model="paper.title" maxlength="512" show-word-limit></el-input>
            </el-form-item>
            <div class="select-warp">
              <el-form-item label="使用该试卷的场次">
                <el-select v-model="selPart" placeholder="请选择场次">
                  <el-option
                    v-for="part in paper.parts"
                    :key="part.id"
                    :label="part.date + '@' + part.title"
                    :value="part.id">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="选择出题标签(非必选)">
                <el-select v-model="selTags" placeholder="请选择标签" :multiple="true">
                  <el-option
                    v-for="tag in tags"
                    :key="tag.id"
                    :label="tag.name"
                    :value="tag.id">
                  </el-option>
                </el-select>
              </el-form-item>
            </div>
            <el-form-item class="max-score" label="总分">
              <el-input
                placeholder="总分"
                v-model="maxScore"
                :disabled="true"
                :class="maxScore !== 100 ? 'error' : ''">
              </el-input>
            </el-form-item>
            <div class="tag-percent-content">
              <el-divider content-position="left" v-if="tagPercent.length != 0">所选标签中的题占总题数的比例</el-divider>
                <el-form-item :label="item.name" v-for="item,index in tagPercent" class="tag-percent-warp" :precision="1">
                  <el-input-number v-model="item.percent" :min="1" :max="100" label="百分比" class="percent-num"></el-input-number>&nbsp;%
                </el-form-item>
              <el-divider></el-divider>
            </div>
            <el-form-item label="选择题">
              <span>
                数量
                <el-input-number v-model="paper.single.num" @change="singleNumChange" :min="0" :max="50" label="选择题数量"></el-input-number>
              </span>
              <span>
                每题分数
                <el-input-number v-model="paper.single.score" @change="singleScoreChange" :min="1" :max="50" label="分数"></el-input-number>
              </span>
              <span class="total-score">该类总分：{{ paper.single.total }}</span>
            </el-form-item>
            <el-form-item label="多选题">
              <span>
                数量
                <el-input-number v-model="paper.multiple.num" @change="multipleNumChange" :min="0" :max="50" label="多选题数量"></el-input-number>
              </span>
              <span>
                每题分数
                <el-input-number v-model="paper.multiple.score" @change="multipleScoreChange" :min="1" :max="50" label="分数"></el-input-number>
              </span>
              <span class="total-score">该类总分：{{ paper.multiple.total }}</span>
            </el-form-item>
            <el-form-item label="判断题">
              <span>
                数量
                <el-input-number v-model="paper.judgment.num" @change="judgmentNumChange" :min="0" :max="50" label="判断题数量"></el-input-number>
              </span>
              <span>
                每题分数
                <el-input-number v-model="paper.judgment.score" @change="judgmentScoreChange" :min="1" :max="50" label="分数"></el-input-number>
              </span>
              <span class="total-score">该类总分：{{ paper.judgment.total }}</span>
            </el-form-item>
            <el-form-item label="填空题">
              <span>
                数量
                <el-input-number v-model="paper.fill.num" @change="fillNumChange" :min="0" :max="50" label="填空题数量"></el-input-number>
              </span>
              <span>
                每题分数
                <el-input-number v-model="paper.fill.score" @change="fillScoreChange" :min="1" :max="50" label="分数"></el-input-number>
              </span>
              <span class="total-score">该类总分：{{ paper.fill.total }}</span>
            </el-form-item>
            <el-form-item class="submit-btn">
              <el-button type="primary" @click="onSubmit">创建试卷</el-button>
            </el-form-item>
          </el-form>
          <el-card class="tips-card">
            <div slot="header" class="clearfix">
              <span>提示</span>
            </div>
            <ul>
               <li>如果选择的标签中题目数量不足，将随机从题库中抽取题目</li>
               <li>请自行计算好出题比例、出题数目和分数</li>
            </ul>
          </el-card>
        </div>
      </div>
    `,
  methods: {
    onSubmit: function () {
      // 获取数据
      var title = this.paper.title;
      var singleNum = this.paper.single.num;
      var singleScore = this.paper.single.score;
      var multipleNum = this.paper.multiple.num;
      var multipleScore = this.paper.multiple.score;
      var judgmentNum = this.paper.judgment.num;
      var judgmentScore = this.paper.judgment.score;
      var fillNum = this.paper.fill.num;
      var fillScore = this.paper.fill.score;
      var tags = this.selTags;
      var partId = this.selPart;
      var tagPercent = JSON.stringify(this.tagPercent);
      var totalScore = this.maxScore;

      // 数据校验
      if (title === '') {
        return this.$message.error("请输入题目");
      }
      if (partId === null) {
        return this.$message.error("请选择场次");
      }
      this.isLoading = true;
      axios.post('/Paper/AddPaper', {
        title,
        singleNum,
        singleScore,
        multipleNum,
        multipleScore,
        judgmentNum,
        judgmentScore,
        fillNum,
        fillScore,
        tags,
        partId,
        tagPercent
      }).then(res => {
        const { data } = res;
        if (data.code === 1) {
          this.isLoading = false;
          return this.$message.error(data.message);
        }

        // 重置paper
        this.paper = {
          title: '',
          parts: this.paper.parts,
          tags: this.paper.tags,
          single: { num: 10, score: 3, total: 30 },
          multiple: { num: 10, score: 2, total: 20 },
          judgment: { num: 10, score: 2, total: 20 },
          fill: { num: 10, score: 3, total: 30 },
        };
        this.selPart = null;
        this.selTags = [];
        this.isLoading = false;
        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    },
    // 改变分数或题数
    singleNumChange: function (val) {
      this.paper.single.num = val;
      this.paper.single.total = this.paper.single.num * this.paper.single.score;
    },
    singleScoreChange: function (val) {
      this.paper.single.score = val;
      this.paper.single.total = this.paper.single.num * this.paper.single.score;
    },
    multipleNumChange: function (val) {
      this.paper.multiple.num = val;
      this.paper.multiple.total = this.paper.multiple.num * this.paper.multiple.score;
    },
    multipleScoreChange: function (val) {
      this.paper.multiple.score = val;
      this.paper.multiple.total = this.paper.multiple.num * this.paper.multiple.score;
    },
    judgmentNumChange: function (val) {
      this.paper.judgment.num = val;
      this.paper.judgment.total = this.paper.judgment.num * this.paper.judgment.score;
    },
    judgmentScoreChange: function (val) {
      this.paper.judgment.score = val;
      this.paper.judgment.total = this.paper.judgment.num * this.paper.judgment.score;
    },
    fillNumChange: function (val) {
      this.paper.fill.num = val;
      this.paper.fill.total = this.paper.fill.num * this.paper.fill.score;
    },
    fillScoreChange: function (val) {
      this.paper.fill.score = val;
      this.paper.fill.total = this.paper.fill.num * this.paper.fill.score;
    },
    // 获取标签
    getTags: function () {
      axios.get('/Tag/GetTags').then(res => {
        var data = res.data;
        tags = data.slice(1, data.length);

        if (localStorage.getItem('id') != 1) {
          this.tags = tags;
        } else {
          this.tags = data;
        }
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

        this.paper.parts = exams;
      });
    }
  },
  watch: {
    selTags: function (val, oldVal) {
      var tempTagPercent = [];
      var tagLen = val.length;
      for (var i in val) {
        for (var j in this.tags) {
          if (this.tags[j].id == val[i]) {
            var targetPercent = Math.floor(100 / tagLen);
            if (targetPercent * tagLen != 100 && i == tagLen - 1) {
              targetPercent = 100 - targetPercent * tagLen + targetPercent;
            }
            tempTagPercent.push({ id: val[i], name: this.tags[j].name, percent: targetPercent })
          }
        }
      }
      this.tagPercent = tempTagPercent;
    }
  },
  created() {
    this.maxScore = this.paper.fill.total + this.paper.judgment.total + this.paper.multiple.total + this.paper.single.total;
    this.getTags();
    this.getExamPart();
  },
  beforeUpdate() {
    this.maxScore = this.paper.fill.total + this.paper.judgment.total + this.paper.multiple.total + this.paper.single.total;
  },
  updated() {
    var currentScore = this.paper.fill.total + this.paper.judgment.total + this.paper.multiple.total + this.paper.single.total;
    if (currentScore > 100) {
      this.$notify.error({
        title: '错误',
        message: '分数已经超出100！'
      });
    } else if (currentScore < 100) {
      this.$notify.error({
        title: '错误',
        message: '分数不足100！'
      });
    }
  }
});