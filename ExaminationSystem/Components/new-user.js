Vue.component('new-user', {
  props: {
  },
  data() {
    return {
      newUser: {
        account: '',
        password: '',
        name: '',
        roleId: '1'
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
      <div class="new-user">
        <div class="main-header">添加用户</div>
        <div class="main-content">
          <el-form :model="newUser" :rules="rules" label-position="right">
            <el-form-item label="账号" prop="account">
              <el-input v-model="newUser.account"></el-input>
            </el-form-item>
            <el-form-item label="密码" prop="password">
              <el-input v-model="newUser.password"></el-input>
            </el-form-item>
            <el-form-item label="姓名" prop="name">
              <el-input v-model="newUser.name"></el-input>
            </el-form-item>
            <el-form-item label="身份" class="role-wrap">
              <el-select v-model="newUser.roleId" placeholder="请选择用户身份">
                <el-option label="出卷人" value="2"></el-option>
                <el-option label="考生" value="1"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button class="new-btn" type="primary" @click="onSubmit">添加</el-button>
            </el-form-item>
          </el-form>
        </div>
      </div>
    `,
  methods: {
    onSubmit() {
      const { account, name, password, roleId } = this.newUser;

      axios.post('/BackEnd/AddUser', { account, name, password, roleId }).then(res => {
        const { data } = res;
        if (data.code == 1) {
          return this.$message.error(data.message);
        }

        this.newUser.account = '';
        this.newUser.name = '';
        this.newUser.password = '';
        this.newUser.roleId = '1';

        this.$emit('is-insert');

        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    }
  },
});