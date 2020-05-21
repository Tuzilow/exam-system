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
        trueSel: '', // 正确答案
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
        trueSel: '', // 正确答案
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
      // 富文本编辑器设置
      editorOption: {
        placeholder: '请输入题目',
        theme: 'snow',
        modules: {
          toolbar: {
            container:
              [['bold', 'italic', 'underline', 'strike'],
              [{ 'header': 1 }, { 'header': 2 }], [{ 'list': 'ordered' }, { 'list': 'bullet' }],
              [{ 'header': [1, 2, 3, false] }],
              [{ 'color': [] }, { 'background': [] }],
              ['image']],
            handlers: {
              'image': function (value) {
                if (value) {
                  document.querySelector('.image-uploader input').click();
                } else {
                  this.quill.format('image', false);
                }
              }
            }
          }
        }
      },
      // 图片上传设置
      serverUrl: '/Image/UpLoad',
      isUploading: false,
      currentEditor: 'singleEditor',
      tags: [
        { id: 1, name: '123' },
        { id: 2, name: 'a' },
        { id: 3, name: 's' },
        { id: 4, name: 'c' },
      ],
      selTags: [], // 选中标签
      isSubmitLoading: false
    };
  },
  template: `
       <div class="new-question">
        <div class="main-header">添加题目</div>
        <div class="main-content"  v-loading="isUploading">
           <el-tabs type="border-card" @tab-click="changeTab">
            <el-tab-pane label="单选题">
              <el-form :model="single" class="single">
                <el-form-item class="title-editor-wrap">
                  <el-alert
                    title="请注意将正确答案输入到指定文本框中"
                    type="warning">
                  </el-alert>
                  <el-upload
                    class="image-uploader"
                    :action="serverUrl"
                    name="img"
                    :show-file-list="false"
                    :on-success="uploadSuccess"
                    :on-error="uploadError"
                    :before-upload="beforeUpload">
                  </el-upload>
                  <quill-editor
                    class="title-editor"
                    ref="singleEditor"
                    :content="single.title"
                    :options="editorOption"
                    @change="onSingleChange($event)"
                  />
                </el-form-item>
                <el-form-item label="正确选项" class="answers true-answer">
                  <el-input v-model.trim="single.trueSel"></el-input>
                </el-form-item>
                <el-form-item label="错误选项1" class="answers false-answer">
                  <el-input v-model.trim="single.sel1"></el-input>
                </el-form-item>
                <el-form-item label="错误选项2" class="answers false-answer">
                  <el-input v-model.trim="single.sel2"></el-input>
                </el-form-item>
                <el-form-item label="错误选项3" class="answers false-answer">
                  <el-input v-model.trim="single.sel3"></el-input>
                </el-form-item>
                <el-form-item class="submit-btn">
                  <el-button type="primary" @click="showScore('single')">确 定</el-button>
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
                    ref="multipleEditor"
                    :content="multiple.title"
                    :options="editorOption"
                    @change="onMultipleChange($event)"
                  />
                </el-form-item>
                <el-form-item label="选项1" class="answers">
                  <el-input v-model.trim="multiple.MQAns1"></el-input>
                </el-form-item>
                <el-form-item label="选项2" class="answers">
                  <el-input v-model.trim="multiple.MQAns2"></el-input>
                </el-form-item>
                <el-form-item label="选项3" class="answers">
                  <el-input v-model.trim="multiple.MQAns3"></el-input>
                </el-form-item>
                <el-form-item label="选项4" class="answers">
                  <el-input v-model.trim="multiple.MQAns4"></el-input>
                </el-form-item>
                <el-form-item label="选项5" class="answers">
                  <el-input v-model.trim="multiple.MQAns5"></el-input>
                </el-form-item>
                <el-form-item label="选项6" class="answers">
                  <el-input v-model.trim="multiple.MQAns6"></el-input>
                </el-form-item>
                <el-form-item label="选项7" class="answers">
                  <el-input v-model.trim="multiple.MQAns7"></el-input>
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
                  <el-button type="primary" @click="showScore('multiple')">确 定</el-button>
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
                    ref="judgmentEditor"
                    :content="judgment.title"
                    :options="editorOption"
                    @change="onJudgmentChange($event)"
                  />
                </el-form-item>
                <el-form-item label="正确选项" class="answers true-answer">
                  <el-input v-model.trim="judgment.trueSel"></el-input>
                </el-form-item>
                <el-form-item label="错误选项" class="answers false-answer">
                  <el-input v-model.trim="judgment.falseSel"></el-input>
                </el-form-item>
                <el-form-item class="submit-btn">
                  <el-button type="primary" @click="showScore('judgment')">确 定</el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
            <el-tab-pane label="填空题">
              <el-form :model="fill" class="fill">
                <el-form-item class="title-editor-wrap fill-editor-wrap">
                  <el-alert
                    title="请手动在题目中留下填写答案的位置，同时请按照正确答案的顺序设置答案。新增的答案框不能删除，请谨慎添加！！！"
                    type="warning">
                  </el-alert>
                  <el-button type="success" @click="addFillAnswer" size="mini" style="width:100%;">添加答案</el-button>
                  <quill-editor
                    class="title-editor fill-editor"
                    ref="fillEditor"
                    :content="fill.title"
                    :options="editorOption"
                    @change="onFillChange($event)"
                  />
                </el-form-item>
                <el-form-item label="答案" class="answers" v-for="(item, index) in fill.answers" :key="index">
                  <el-input v-model.trim="fill.answers[index]"></el-input>
                </el-form-item>
                <el-form-item class="submit-btn">
                  <el-button type="primary" @click="showScore('fill')">确 定</el-button>
                </el-form-item>
              </el-form>
            </el-tab-pane>
          </el-tabs>
        </div>
        <el-dialog
          title="确认参考分数并选择标签"
          :visible="isShowDialog"
          @close="isShowDialog = false"
          v-loading="isSubmitLoading">
          <el-alert
            title="可以不选择标签直接保存"
            type="error">
          </el-alert>
          <div class="submit-dialog">
            <el-input-number v-model="currentScore" @change="scoreChange" :min="1" :max="10" class="score-change"></el-input-number>
            <el-select v-model="selTags" filterable multiple placeholder="请选择标签" class="select-tag">
              <el-option
                v-for="item in tags"
                :key="item.id"
                :label="item.name"
                :value="item.id">
              </el-option>
            </el-select>
          </div>
          <span slot="footer" class="dialog-footer">
            <el-button @click="isShowDialog = false">取 消</el-button>
            <el-button type="primary" @click="onSubmit">提 交</el-button>
          </span>
        </el-dialog>
      </div>
    `,
  methods: {
    // 显示修改分数界面
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
    // 切换tab时切换分数
    scoreChange: function (val) {
      switch (this.currentType) {
        case 'single': this.single.score = val; break;
        case 'multiple': this.multiple.score = val; break;
        case 'judgment': this.judgment.score = val; break;
        case 'fill': this.fill.score = val; break;
      }
    },
    onSubmit: function () {
      this.isSubmitLoading = true;
      // 获取要提交的数据
      let submitData;
      switch (this.currentType) {
        case 'single': submitData = this.getSingle(); break;
        case 'multiple': submitData = this.getMultiple(); break;
        case 'judgment': submitData = this.getJudgment(); break;
        case 'fill': submitData = this.getFill(); break;
      }
      // 获取选中的标签
      let tagsId = this.selTags;
      submitData = { ...submitData, tagsId };

      switch (this.currentType) {
        case 'single': this.addSingle(submitData); break;
        case 'multiple': this.addMultiple(submitData); break;
        case 'judgment': this.addJudgment(submitData); break;
        case 'fill': this.addFill(submitData); break;
      }

    },
    // 获取题目
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
    // 添加填空题答案
    addFillAnswer: function () {
      this.fill.answers.push('');
    },
    // 上传图片前
    beforeUpload: function () {
      this.isUploading = true;
    },
    // 上传图片成功
    uploadSuccess: function (res, file) {
      // res为图片服务器返回的数据
      // 获取富文本组件实例
      let quill;
      switch (this.currentEditor) {
        case 'singleEditor': quill = this.$refs.singleEditor.quill; break;
        case 'multipleEditor': quill = this.$refs.multipleEditor.quill; break;
        case 'judgmentEditor': quill = this.$refs.judgmentEditor.quill; break;
        case 'fillEditor': quill = this.$refs.fillEditor.quill; break;
      }
      // 如果上传成功
      if (res.code === 0 && res.path !== null) {
        // 获取光标所在位置
        let length = quill.getSelection().index;
        // 插入图片  res.info为服务器返回的图片地址
        quill.insertEmbed(length, 'image', res.path);
        // 调整光标到最后
        quill.setSelection(length + 1)
      } else {
        this.$message.error(res.message);
      }
      // loading动画消失
      this.isUploading = false
    },
    // 上传图片失败
    uploadError: function () {
      // loading动画消失
      this.isUploading = false
      this.$message.error(res.message);
    },
    // 改变当前Editor
    changeTab: function (val) {
      this.selTags = [];
      switch (val.index) {
        case '0': this.currentEditor = 'singleEditor'; break;
        case '1': this.currentEditor = 'multipleEditor'; break;
        case '2': this.currentEditor = 'judgmentEditor'; break;
        case '3': this.currentEditor = 'fillEditor'; break;
      }
    },
    // 获得全部标签
    getTags: function () {
      this.isLoading = true;
      axios.get('/Tag/GetTags').then(res => {
        this.tags = res.data;
        this.totalNum = res.data.length || 0;
        this.isLoading = false;
      });
    },
    // 获取要提交的单选数据
    getSingle: function () {
      let { title, sel1, sel2, sel3, trueSel, score } = this.single;

      if (title === '' || trueSel === '' || sel1 === '' || sel2 === '' || sel3 === '') {
        this.$message({
          type: 'error',
          message: '请将所有内容填写完整后提交'
        });
        return null;
      }

      return { title, trueSel, sel1, sel2, sel3, score };
    },
    // 获取多选
    getMultiple: function () {
      let { title, MQAns1, MQAns2, MQAns3, MQAns4, MQAns5, MQAns6, MQAns7, trueSels, score } = this.multiple;

      if (title === '' || MQAns1 === '' || MQAns2 === '' || MQAns3 === '' || MQAns4 === '') {
        this.$message({
          type: 'error',
          message: '请至少填写题目和四个答案'
        });
        return null;
      }

      let newSels = trueSels.map((item) => {
        return item.replace('选项', 'MQAns');
      });

      return { title, answers: [MQAns1, MQAns2, MQAns3, MQAns4, MQAns5, MQAns6, MQAns7], trueSels: newSels, score };
    },
    // 获取判断
    getJudgment: function () {
      let { title, trueSel, falseSel, score } = this.judgment;

      if (title === '' || trueSel === '' || falseSel === '') {
        this.$message({
          type: 'error',
          message: '请将所有内容填写完整后提交'
        });
        return null;
      }

      return { title, trueSel, falseSel, score };
    },
    // 获取填空
    getFill: function () {
      let { title, score, answers } = this.fill;

      if (title === '' || answers[0] === '') {
        this.$message({
          type: 'error',
          message: '请将所有内容填写完整后提交'
        });
        return null;
      }

      let newAnswers = answers.filter((item) => {
        return item !== '';
      });

      return { title, score, answers: newAnswers };
    },
    // 添加单选题
    addSingle: function (data) {
      axios.post('/Question/AddSingle', { ...data }).then(res => {
        this.isSubmitLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }

        this.single = {
          title: '',
          sel1: '',
          sel2: '',
          sel3: '',
          trueSel: '',
          score: 2
        };
        this.selTags = [];
        this.isShowDialog = false;
        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    },
    // 添加多选题
    addMultiple: function (data) {
      axios.post('/Question/AddMultiple', { ...data }).then(res => {
        this.isSubmitLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }

        this.multiple = {
          title: '',
          MQAns1: '',
          MQAns2: '',
          MQAns3: '',
          MQAns4: '',
          MQAns5: '',
          MQAns6: '',
          MQAns7: '',
          trueSels: ['选项1', '选项2'],
          score: 2
        };
        this.selTags = [];
        this.isShowDialog = false;
        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    },
    // 添加判断题
    addJudgment: function (data) {
      axios.post('/Question/AddJudgment', { ...data }).then(res => {
        this.isSubmitLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }

        this.judgment = {
          title: '',
          trueSel: '',
          falseSel: '',
          score: 2
        }
        this.selTags = [];
        this.isShowDialog = false;
        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    },
    addFill: function (data) {
      axios.post('/Question/AddFill', { ...data }).then(res => {
        this.isSubmitLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }

        this.fill = {
          title: '',
          answers: [''],
          score: 2
        }
        this.selTags = [];
        this.isShowDialog = false;
        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    }
  },
  created() {
    this.getTags();
  }
});