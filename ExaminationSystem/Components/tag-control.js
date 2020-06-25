Vue.component('tag-control', {
  props: {
  },
  data() {
    return {
      tags: [],
      totalNum: 1,
      tagType: ['', 'success', 'info', 'danger', 'warning'],
      isLoading: false,
      editLoading: false,
      isShowInfo: false,
      dialogTitle: '添加新标签',
      tag: {
        id: 0,
        name: '',
        desc: ''
      },
      pageSize: 10,
      currentPage: 1
    }
  },
  template: `
        <div>
          <div>
            <div class="main-header">标签管理</div>
            <div class="tag-cloud">
              <div>
                <el-tag
                  v-for="tag in tags"
                  :key="tag.name"
                  effect="dark"
                  closable
                  @close="remove(tag)"
                  :type="tagType[tag.id % 5]">
                  {{tag.name}}
                </el-tag>
                <el-button class="add-tag" size="small" @click="showAdd">+ 添加新标签</el-button>
              </div>
            </div>
            <el-table :data="tags.slice((currentPage-1)*pageSize,currentPage*pageSize)" v-loading="isLoading">
              <el-table-column prop="name" label="标签名"></el-table-column>
              <el-table-column prop="desc" label="描述"></el-table-column>
              <el-table-column label="操作" align="center" prop="id">
                <template slot-scope="scope">
                  <el-link type="warning" @click="editTag(scope.row)">编辑</el-link>
                  <el-link type="danger" @click="remove(scope.row)">删除</el-link>
                </template>
              </el-table-column>
            </el-table>
            <div class="pagination clearfix">
              <el-pagination background layout="prev, pager, next" :page-size="10" :total="totalNum" @current-change="changePage">
              </el-pagination>
            </div>
          </div>
          <el-dialog :title="dialogTitle" :visible="isShowInfo" @close="isShowInfo = false">
            <el-form :model="tag">
              <el-form-item label="标签名">
                <el-input v-model="tag.name" maxlength="8" show-word-limit></el-input>
              </el-form-item>
              <el-form-item label="描述">
                <el-input v-model="tag.desc" type="textarea" :rows="2" maxlength="64" show-word-limit></el-input>
              </el-form-item>
            </el-form>
            <div slot="footer" class="dialog-footer">
              <el-button @click="isShowInfo = false">取 消</el-button>
              <el-button type="primary" @click="addTag" v-if="dialogTitle==='添加新标签'">添 加</el-button>
              <el-button type="primary" @click="submitEdit" v-if="dialogTitle==='编辑标签信息'">确 定</el-button>
            </div>
          </el-dialog>
        </div>
    `,
  methods: {
    changePage: function (val) {
      this.currentPage = val;
    },
    // 打开编辑界面
    editTag: function (row) {
      this.dialogTitle = '编辑标签信息';

      this.tag.id = row.id;
      this.tag.name = row.name;
      this.tag.desc = row.desc;

      this.isShowInfo = true;
    },
    // 提交编辑
    submitEdit: function () {
      if (this.tag.name == '') {
        return this.$message.error('请填写标签名');
      }
      axios.post('/Tag/EditTag', {
        id: this.tag.id,
        tagName: this.tag.name,
        desc: this.tag.desc
      }).then(res => {
        const { data } = res;
        if (data.code == 1) {
          return this.$message.error(data.message);
        }

        this.tag.name = '';
        this.tag.desc = '';

        this.getTags();
        this.isShowInfo = false;

        return this.$message({
          message: data.message,
          type: 'success'
        });
      });
    },
    remove: function (tag) {
      console.log(tag)
      this.$confirm('确认删除此记录?', '警告', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        axios.post('/Tag/RemoveTag', { id: tag.id }).then(res => {
          const { data } = res;
          if (data.code == 1) {
            return this.$message.error(data.message);
          }

          this.getTags();
          this.isShowInfo = false;
          return this.$message({
            message: data.message,
            type: 'success'
          });
        });
      }).catch(() => {
        this.$message({
          type: 'info',
          message: '已取消删除'
        });
      });
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
    // 打开添加界面
    showAdd: function () {
      this.isShowInfo = true;
      this.tag.name = '';
      this.tag.desc = '';
      this.dialogTitle = '添加新标签';
    },
    // 提交添加
    addTag: function () {
      if (this.tag.name == '') {
        return this.$message.error('请填写标签名称');
      }
      axios.post('/Tag/AddTag', {
        tagName: this.tag.name,
        desc: this.tag.desc
      }).then(res => {
        const { data } = res;
        if (data.code == 1) {
          return this.$message.error(data.message);
        }

        this.tag.name = '';
        this.tag.desc = '';

        this.getTags();
        this.isShowInfo = false;
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