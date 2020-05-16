Vue.component('new-question', {
  props: {
  },
  data() {
    return {
      // 单选题
      single: {
        title: '',
        sel1: '',
        sel2: '',
        sel3: '',
        tureSel: '', // 正确答案
        score: 2
      },
      // 多选题
      multiple: {
        title: '',
        MQAns1: '',
        MQAns2: '',
        MQAns3: '',
        MQAns4: '',
        MQAns5: '',
        MQAns6: '',
        MQAns7: '',
        trueSels: ['选项1', '选项2'], // 正确选项
        score: 2
      },
      // 判断题
      judgment: {
        title: '',
        tureSel: '', // 正确答案
        falseSel: '', // 错误答案
        score: 2
      },
      // 填空题
      fill: {
        title: '',
        answers: [''], // 答案array
        score: 2
      },
      isShowDialog: false,
      currentScore: 2,
      currentType: 'single',
      editorOption: {
        placeholder: '请输入题目',
        theme: 'snow',
        modules: {
          toolbar: [
            ['bold', 'italic', 'underline', 'strike'],
            [{ 'header': 1 }, { 'header': 2 }], [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'header': [1, 2, 3, false] }],
            [{ 'color': [] }, { 'background': [] }],
            ['image']
          ]
        }
      }
    };
  },
  template: `
       <div class="new-paper">
        <div class="main-header">添加题目</div>
        <div class="main-content">
           <el-tabs type="border-card">
            <el-tab-pane label="单选题">
              <el-form :model="single" class="single">
                <el-form-item class="title-editor-wrap">
                  <el-alert
                    title="请注意将正确答案输入到指定文本框中"
                    type="warning">
                  </el-alert>
                  <quill-editor
                    class="title-editor"
                    :content="single.title"
                    :options="editorOption"
                    @change="onSingleChange($event)"
                  />
                </el-form-item>
                <el-form-item label="正确选项" class="answers true-answer">
                  <el-input v-model="single.tureSel"></el-input>
                </el-form-item>
                <el-form-item label="错误选项1" class="answers false-answer">
                  <el-input v-model="single.sel1"></el-input>
                </el-form-item>
                <el-form-item label="错误选项2" class="answers false-answer">
                  <el-input v-model="single.sel2"></el-input>
                </el-form-item>
                <el-form-item label="错误选项3" class="answers false-answer">
                  <el-input v-model="single.sel3"></el-input>
                </el-form-item>
                <el-form-item class="submit-btn">
                  <el-button type="primary" @click="showScore('single')">提交</el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
            <el-tab-pane label="多选题">
              <el-form :model="multiple" class="multiple">
                <el-form-item class="title-editor-wrap">
                  <el-alert
                    title="最多七个选项，至少填写前四个选项，且至少有两个正确选项。如果选项为空，则无法被选择为正确选项"
                    type="warning">
                  </el-alert>
                  <quill-editor
                    class="title-editor"
                    :content="multiple.title"
                    :options="editorOption"
                    @change="onMultipleChange($event)"
                  />
                </el-form-item>
                <el-form-item label="选项1" class="answers">
                  <el-input v-model="multiple.MQAns1"></el-input>
                </el-form-item>
                <el-form-item label="选项2" class="answers">
                  <el-input v-model="multiple.MQAns2"></el-input>
                </el-form-item>
                <el-form-item label="选项3" class="answers">
                  <el-input v-model="multiple.MQAns3"></el-input>
                </el-form-item>
                <el-form-item label="选项4" class="answers">
                  <el-input v-model="multiple.MQAns4"></el-input>
                </el-form-item>
                <el-form-item label="选项5" class="answers">
                  <el-input v-model="multiple.MQAns5"></el-input>
                </el-form-item>
                <el-form-item label="选项6" class="answers">
                  <el-input v-model="multiple.MQAns6"></el-input>
                </el-form-item>
                <el-form-item label="选项7" class="answers">
                  <el-input v-model="multiple.MQAns7"></el-input>
                </el-form-item>
                <el-form-item label="正确选项" class="answers">
                  <el-checkbox-group v-model="multiple.trueSels" :min="2">
                    <el-checkbox label="选项1" border :disabled="multiple.MQAns1 === ''"></el-checkbox>
                    <el-checkbox label="选项2" border :disabled="multiple.MQAns2 === ''"></el-checkbox>
                    <el-checkbox label="选项3" border :disabled="multiple.MQAns3 === ''"></el-checkbox>
                    <el-checkbox label="选项4" border :disabled="multiple.MQAns4 === ''"></el-checkbox>
                    <el-checkbox label="选项5" border :disabled="multiple.MQAns5 === ''"></el-checkbox>
                    <el-checkbox label="选项6" border :disabled="multiple.MQAns6 === ''"></el-checkbox>
                    <el-checkbox label="选项7" border :disabled="multiple.MQAns7 === ''"></el-checkbox>
                  </el-checkbox-group>
                </el-form-item>
                <el-form-item class="submit-btn">
                  <el-button type="primary" @click="showScore('multiple')">提交</el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
            <el-tab-pane label="判断题">
              <el-form :model="judgment" class="judgment">
                <el-form-item class="title-editor-wrap">
                  <el-alert
                    title="请注意将正确答案输入到指定文本框中"
                    type="warning">
                  </el-alert>
                  <quill-editor
                    class="title-editor"
                    :content="judgment.title"
                    :options="editorOption"
                    @change="onJudgmentChange($event)"
                  />
                </el-form-item>
                <el-form-item label="正确选项" class="answers true-answer">
                  <el-input v-model="judgment.tureSel"></el-input>
                </el-form-item>
                <el-form-item label="错误选项" class="answers false-answer">
                  <el-input v-model="judgment.falseSel"></el-input>
                </el-form-item>
                <el-form-item class="submit-btn">
                  <el-button type="primary" @click="showScore('judgment')">提交</el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
            <el-tab-pane label="填空题">
              <el-form :model="fill" class="fill">
                <el-form-item class="title-editor-wrap">
                  <el-button type="success" @click="addFillAnswer" size="mini" style="width:100%;">添加答案</el-button>
                  <quill-editor
                    class="title-editor"
                    :content="fill.title"
                    :options="editorOption"
                    @change="onFillChange($event)"
                  />
                </el-form-item>
                <el-form-item label="答案" class="answers" v-for="(item, index) in fill.answers" :key="index">
                  <el-input v-model="fill.answers[index]"></el-input>
                </el-form-item>
                <el-form-item class="submit-btn">
                  <el-button type="primary" @click="showScore('fill')">提交</el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
          </el-tabs>
        </div>
        <el-dialog
          title="确认分数"
          :visible="isShowDialog"
          @close="isShowDialog = false">
          <el-alert
            title="为保证同类型题目分数相同，请尽量不要修改分数"
            type="error">
          </el-alert>
          <el-input-number v-model="currentScore" @change="scoreChange" :min="1" :max="10" class="score-change"></el-input-number>
          <span slot="footer" class="dialog-footer">
            <el-button @click="isShowDialog = false">取 消</el-button>
            <el-button type="primary" @click="onSubmit">确定分数并提交</el-button>
          </span>
        </el-dialog>
      </div>
    `,
  methods: {
    showScore: function (type) {
      this.currentType = type;
      let score;
      switch (type) {
        case 'single': score = this.single.score; break;
        case 'multiple': score = this.multiple.score; break;
        case 'judgment': score = this.judgment.score; break;
        case 'fill': score = this.fill.score; break;
      }
      this.currentScore = score;
      this.isShowDialog = true;
    },
    scoreChange: function (val) {
      switch (this.currentType) {
        case 'single': this.single.score = val; break;
        case 'multiple': this.multiple.score = val; break;
        case 'judgment': this.judgment.score = val; break;
        case 'fill': this.fill.score = val; break;
      }
    },
    onSubmit: function () {

    },
    onSingleChange: function (val) {
      this.single.title = val.html;
    },
    onMultipleChange: function (val) {
      this.multiple.title = val.html;
    },
    onJudgmentChange: function (val) {
      this.judgment.title = val.html;
    },
    onFillChange: function (val) {
      this.fill.title = val.html;
    },
    addFillAnswer: function () {
      this.fill.answers.push('');
    }
  },
});