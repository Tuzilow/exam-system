﻿Vue.component('new-user', {
  props: {
  },
  data() {
    return {
      newUser: {
        account: '',
        password: '',
        name: '',
        roleId: '1',

        fileList: []

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
      },
      isLoading: false
    }
  },
  template: `
      <div class="new-user">
        <div class="main-header">添加用户</div>
        <div class="main-content" v-loading="isLoading">
          <div>
            <el-form :model="newUser" :rules="rules" label-position="right">
              <el-form-item label="账号" prop="account">
                <el-input v-model="newUser.account"></el-input>
              </el-form-item>
              <el-form-item label="密码" prop="password">
                <el-input v-model="newUser.password" oninput="value=value.replace(/[\u4E00-\u9FA5]/g,'')"></el-input>
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
              <el-form-item class="upload-warp">
                <el-button class="new-btn" type="primary" @click="onSubmit">添加</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div>
            <el-card class="reg-card">
              <div slot="header" class="clearfix">
                <span>批量上传规则</span>
              </div>
              <div class="content-warp">
                <ul>
                  <li>请使用text文本文件上传</li>
                  <li>用户数据格式为：账号,密码,姓名</li>
                  <li>用户数据格式之间的逗号为英文逗号</li>
                </ul>
                <p class="example">示例：</p>
                <div class="reg-content">
                  <p>19aa22a,123456,测试用户a</p>
                  <p>19aa22b,123456,测试用户b</p>
                  <p>19aa22c,123456,测试用户c</p>
                </div>
              </div>
            </el-card>
            <el-upload
              class="upload-demo"
              action="/Text/Upload"
              list-type="text"
              multiple
              :on-success="fileUploadSuccess"
              :before-upload="beforeUpload">
              <el-button size="small" type="info">通过文件批量添加</el-button>
              <div slot="tip" class="el-upload__tip">只能上传后缀为.txt的文件</div>
            </el-upload>
          </div>
        </div>
      </div>
    `,
  methods: {
    onSubmit() {
      const { account, name, password, roleId } = this.newUser;

      if (account === '' || name === '' || password === '') {
        return this.$alert('请检查数据是否输入完整', '提示', {
          confirmButtonText: '确定',
          type: 'warning'
        });
      }

      this.isLoading = true;
      axios.post('/BackEnd/AddUser', { account, name, password, roleId }).then(res => {
        this.isLoading = false;
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
    },
    beforeUpload(file) {
      const isText = file.type === 'text/plain';
      if (!isText) {
        this.$message.error('上传的文件只能是 txt 格式!');
        return false;
      }
    },
    fileUploadSuccess(res) {
      if (res.code === 1) {
        return this.$message.error(res.message);
      }
      return this.$message({
        message: res.message,
        type: 'success'
      })
    }
  },
});