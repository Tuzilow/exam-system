Vue.component('exam-table', {
  props: {
  },
  data() {
    return {
      exams: [
        { id: 1, start: '2020-05-14 08:00', end: '2020-05-14 10:00' }
      ],
      totalNum: 1
    }
  },
  template: `
       <div>
          <div>
        <div class="main-header">场次列表</div>
        <div class="search-wrap">
          <div>
            暂时不知道放啥
          </div>
        </div>
        <el-table :data="exams">
          <el-table-column prop="id" label="ID"></el-table-column>
          <el-table-column prop="start" label="开始时间"></el-table-column>
          <el-table-column prop="end" label="结束时间"></el-table-column>
          <el-table-column label="操作" align="center" prop="id">
            <template slot-scope="scope">
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
  methods: {},
});