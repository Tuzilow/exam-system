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
      default: '0',
    },
  },
  data() {
    return {
      isShowDialog: false,
      editUser: {
        id: 0,
        account: '',
        name: '',
        password: '',
        roleId: 0
      },
      rules: {
        account: [
          { required: true, message: '请输入账号', trigger: 'blur' },
          { min: 1, max: 16, message: '长度在 1 到 16 个字符', trigger: 'blur' }
        ],
        password: [
          { required: true, message: '请输入密码', trigger: 'blur' },
          { min: 1, max: 32, message: '长度在 1 到 32 个字符', trigger: 'blur' }
        ],
        name: [
          { required: true, message: '请输入姓名', trigger: 'blur' },
          { min: 1, max: 8, message: '长度在 1 到 8 个字符', trigger: 'blur' }
        ]
      }
    }
  },
  template: `
       <div>
        <div class="main-header">用户列表</div>
        <div class="search-wrap">
          <el-input :value="searchkey" placeholder="请输入姓名" @input="inputChange"></el-input>
          <el-button type="primary" @click="search">搜索</el-button>
        </div>
        <el-table :data="users">
          <el-table-column prop="id" label="ID"></el-table-column>
          <el-table-column prop="account" label="账号"></el-table-column>
          <el-table-column prop="password" label="密码"></el-table-column>
          <el-table-column prop="name" label="姓名"></el-table-column>
          <el-table-column prop="role" label="身份"></el-table-column>
          <el-table-column label="操作" width="120" prop="id">
            <template slot-scope="scope">
              <el-link type="warning" @click="edit(scope.row)">编辑</el-link>
              <el-link type="danger" @click="remove(scope.row.id)">删除</el-link>
            </template>
          </el-table-column>
        </el-table>
        <div class="pagination clearfix">
          <el-pagination background layout="prev, pager, next" :page-size="10" :total="totalnum" @current-change="onChangePage">
          </el-pagination>
        </div>
        <el-dialog title="编辑用户信息" :visible="isShowDialog" class="edit-user" :show-close="false">
          <el-form :model="editUser" :rules="rules">
            <el-form-item label="ID">
              <el-input v-model="editUser.id" :disabled="true"></el-input>
            </el-form-item>
            <el-form-item label="账号" prop="account">
              <el-input v-model="editUser.account"></el-input>
            </el-form-item>
            <el-form-item label="密码" prop="password">
              <el-input v-model="editUser.password"></el-input>
            </el-form-item>
            <el-form-item label="姓名" prop="name">
              <el-input v-model="editUser.name"></el-input>
            </el-form-item>
            <el-form-item label="身份" class="role-wrap">
              <el-select v-model="editUser.roleId">
                <el-option label="出卷人" value="2"></el-option>
                <el-option label="考生" value="1"></el-option>
              </el-select>
            </el-form-item>
          </el-form>
          <div slot="footer" class="dialog-footer">
            <el-button @click="isShowDialog = false" size="small">取 消</el-button>
            <el-button type="primary" @click="finishEdit" size="small">确 定</el-button>
          </div>
        </el-dialog>
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
    },
    remove: function (id) {
      this.$emit('remove', id);
    },
    edit: function (val) {
      const { id, account, password, name, role } = val;
      this.isShowDialog = true;
      const roleId = role === '考生' ? '1' : role === '出卷人' ? '2' : '3';

      this.editUser.id = id;
      this.editUser.account = account;
      this.editUser.password = password;
      this.editUser.name = name;
      this.editUser.roleId = roleId;
    },

    finishEdit: function () {
      const { id, account, password, name, roleId } = this.editUser;
      axios.post('/BackEnd/EditUser', { id, account, name, password, roleId }).then(res => {
        const { data } = res;
        if (data.code == 1) {
          return this.$message.error(data.message);
        }

        this.$emit('is-insert');
        this.isShowDialog = false;

        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    }
  },
});