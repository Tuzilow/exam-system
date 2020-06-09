Vue.component('new-exam', {
  props: {
  },
  data() {
    return {
      exam: {
        date: new Date().toLocaleDateString(),
        start: new Date(new Date().toLocaleDateString() + ' 08:00:00').getTime(),
        end: '',
        time: 120
      },
      users: [],
      selectUsers: [],
      isLoading: true,
      errorUserMsg: [],
      showErrUser: false
    }
  },
  template: `
       <div class="new-exam">
        <div class="main-header">添加场次</div>
        <el-alert
          title="添加场次后请及时为该场次添加考试试卷！！！"
          type="error">
        </el-alert>
        <div class="main-content"  v-loading="isLoading">
          <el-form :model="exam"  label-position="left" class="form">
            <el-form-item label="日期" prop="date">
              <el-date-picker
                class="content"
                v-model="exam.date"
                type="date"
                placeholder="选择日期">
              </el-date-picker>
            </el-form-item>
            <el-form-item label="开始时间" prop="start">
              <el-time-picker
                class="content"
                v-model="exam.start"
                placeholder="开始时间"
                :picker-options="{
                  selectableRange: '08:00:00 - 19:00:00'
                }"
                @change="changeTime"
              >
              </el-time-picker>
            </el-form-item>
            <el-form-item label="考试时长(分钟)" prop="time">
              <el-slider
                class="content"
                v-model="exam.time"
                :min="60"
                :max="150"
                :step="10"
                @change="changeTime"
              >
              </el-slider>
            </el-form-item>
            <el-form-item label="结束时间" prop="end">
              <el-time-picker
                class="content"
                v-model="exam.end"
                disabled
                placeholder="结束时间">
              </el-time-picker>
            </el-form-item>
            <el-form-item class="transfer" label-position="top" label="请选择该场次的考生">
              <el-transfer
                filterable
                :titles="['可选考生', '已选考生']"
                v-model="selectUsers"
                :data="users">
              </el-transfer>
            </el-form-item>
            <el-form-item class="submit-btn">
              <el-button class="new-btn" type="primary" @click="onSubmit">添加</el-button>
            </el-form-item>
          </el-form>
        </div>
        <el-dialog title="考试时间冲突学生" :visible.sync="showErrUser" width="70%">
          <el-table :data="errorUserMsg">
            <el-table-column type="expand">
              <template slot-scope="props">
                <el-form label-position="left" inline class="demo-table-expand">
                  <el-form-item :label="'场次' + (index + 1)" v-for="item,index in props.row.haveParts" :key="index">
                    <span> 开始时间：{{item.start}} --- 结束时间：{{item.end}}</span>
                  </el-form-item>
                </el-form>
              </template>
            </el-table-column>
            <el-table-column property="key" label="ID"></el-table-column>
            <el-table-column property="label" label="姓名"></el-table-column>
          </el-table>
        </el-dialog>
      </div>
    `,
  methods: {
    // 获得结束时间
    changeTime: function () {
      this.exam.end = new Date(this.exam.start).getTime() + this.exam.time * 60000;
    },
    // 提交添加场次
    onSubmit: function () {
      if (this.selectUsers.length === 0) {
        return this.$alert('请选择加入该场次的考生', '提示', {
          confirmButtonText: '确定',
          type: 'warning'
        });
      }
      let { date } = this.exam;
      let start = new Date(this.exam.start);
      let end = new Date(this.exam.end);
      let userIds = this.selectUsers;
      let selected = [];

      for (var i = 0; i < this.selectUsers.length; i++) {
        for (var j = 0; j < this.users.length; j++) {
          if (this.users[j].key == this.selectUsers[i]) {
            selected.push(this.users[j]);
            break;
          }
        }
      }

      let startTime = new Date(new Date(date).toLocaleDateString() + ' ' + start.getHours() + ':' + start.getMinutes() + ':' + start.getSeconds()).getTime();
      let endTime = new Date(new Date(date).toLocaleDateString() + ' ' + end.getHours() + ':' + end.getMinutes() + ':' + end.getSeconds()).getTime();

      let errUsers = [];
      for (var i = 0; i < selected.length; i++) {
        var tempUser = selected[i]
        for (var j = 0; j < tempUser.haveParts.length; j++) {
          var errStart = new Date(tempUser.haveParts[j].start).getTime();
          var errEnd = new Date(tempUser.haveParts[j].end).getTime();
          console.log(`errStart:${errStart}  errEnd:${errEnd}\nstartTime:${startTime}  endTime:${endTime}`)
          if (errStart <= startTime && errEnd >= endTime) {
            errUsers.push(tempUser);
            break;
          }
          if (errEnd >= startTime && errEnd <= endTime) {
            errUsers.push(tempUser);
            break;
          }
          if (errStart >= startTime && errStart <= endTime) {
            errUsers.push(tempUser);
            break;
          }
          if (errStart >= startTime && errEnd <= endTime) {
            errUsers.push(tempUser);
            break;
          }
        }
      }
      if (errUsers.length > 0) {
        this.showErrUser = true;
        this.errorUserMsg = errUsers;
        return false;
      }

      this.isLoading = true;
      axios.post('/ExamPart/CreateNewPart', {
        start: startTime,
        end: endTime,
        userIds
      }).then(res => {
        this.isLoading = false;
        const { data } = res;
        if (data.code === 1) {
          return this.$message.error(data.message);
        }
        this.selectUsers = [];
        this.getUser();

        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    },
    // 获取可选考生
    getUser: function () {
      this.isLoading = true;
      axios.get('/ExamPart/GetUser').then(res => {
        this.users = res.data;
        this.isLoading = false;
      });
    }
  },
  created() {
    this.changeTime();
    this.getUser();
  }
});