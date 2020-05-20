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
        trueSels: ['MQAns1', 'MQAns2'], // 正确选项
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
        answers: ['aa'], // 答案array
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
        <div v-loading="isLoading">
          <el-table :data="singles" v-if="currentType === '单选题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目"></el-table-column>
            <el-table-column prop="trueSel" label="正确答案"></el-table-column>
            <el-table-column prop="sel1" label="错误答案"></el-table-column>
            <el-table-column prop="sel2" label="错误答案"></el-table-column>
            <el-table-column prop="sel3" label="错误答案"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
            <el-table-column label="操作" prop="id">
              <template slot-scope="scope" v-if="currentType === '单选题'">
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="multiples" v-if="currentType === '多选题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目"></el-table-column>
            <el-table-column prop="MQAns1" label="选项1"></el-table-column>
            <el-table-column prop="MQAns2" label="选项2"></el-table-column>
            <el-table-column prop="MQAns3" label="选项3"></el-table-column>
            <el-table-column prop="MQAns4" label="选项4"></el-table-column>
            <el-table-column prop="MQAns5" label="选项5"></el-table-column>
            <el-table-column prop="MQAns6" label="选项6"></el-table-column>
            <el-table-column prop="MQAns7" label="选项7"></el-table-column>
            <el-table-column prop="trueSels" label="正确选项"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
            <el-table-column label="操作" prop="id">
              <template slot-scope="scope" v-if="currentType === '多选题'">
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="judgments" v-if="currentType === '判断题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目"></el-table-column>
            <el-table-column prop="trueSel" label="正确答案"></el-table-column>
            <el-table-column prop="falseSel" label="错误答案"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
            <el-table-column label="操作" prop="id">
              <template slot-scope="scope" v-if="currentType === '判断题'">
                <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
              </template>
            </el-table-column>
          </el-table>
          <el-table :data="fills" v-if="currentType === '填空题'">
            <el-table-column prop="id" label="ID"></el-table-column>
            <el-table-column prop="title" label="题目"></el-table-column>
            <el-table-column prop="answers" label="答案"></el-table-column>
            <el-table-column prop="score" label="分数"></el-table-column>
            <el-table-column label="操作" prop="id">
              <template slot-scope="scope" v-if="currentType === '填空题'">
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
    remove: function () {

    },
    onChangePage: function (val) {
      this.currentPage = val;
      this.getSingle();
    },
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
        console.log(count, singles)
        this.totalNum = count[0];
        this.singles = singles;
      });
    }
  },
  created() {
    this.getSingle(); // TODO 限制答案显示长度
  }
});