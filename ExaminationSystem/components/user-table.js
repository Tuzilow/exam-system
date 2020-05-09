Vue.component('user-table', {
  props: {
    users: {
      type: Array,
      default: [],
    },
    searchkey: {
      type: String,
      default: '',
    },
    totalnum: {
      type: Number,
      default: 0,
    },
  },
  template: `
       <div>
        <div class="main-header">用户列表</div>
        <div class="search-wrap">
          <el-input :value="searchkey" placeholder="请输入姓名" @input="inputChange"></el-input>
          <el-button type="primary" @click="search">搜索</el-button>
        </div>
        <el-table :data="users" :default-sort="{prop: 'id', order: 'ascending'}">
          <el-table-column prop="id" label="ID" sortable width="180">
          </el-table-column>
          <el-table-column prop="account" label="账号">
          </el-table-column>
          <el-table-column prop="password" label="密码">
          </el-table-column>
          <el-table-column prop="name" label="姓名">
          </el-table-column>
          <el-table-column prop="role" label="身份" sortable>
          </el-table-column>
          <el-table-column label="操作" width="120">
            <el-link type="warning">编辑</el-link>
            <el-link type="danger">删除</el-link>
          </el-table-column>
        </el-table>
        <div class="pagination clearfix">
          <el-pagination background layout="prev, pager, next" :page-size="10" :total="totalnum" @current-change="onChangePage">
          </el-pagination>
        </div>
      </div>
    `,
  methods: {
    onChangePage: function (val) {
      this.$emit('current-page', val);
    },
    inputChange: function (val) {
      this.$emit('input-change', val);
    },
    search: function () {
      this.$emit('search');
    }
  },
});