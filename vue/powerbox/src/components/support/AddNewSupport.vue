<template>
  <v-row justify="center">
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
      persistent
      max-width="600px"
    >
      <template v-can="'Support_Create'" v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on">افزودن </v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">{{
            !supportId ? "افزودن پشتیبان" : "ویرایش پشتیبان"
          }}</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="4">
                  <v-select
                    v-model="selectedComplexId"
                    :items="complexes"
                    item-value="id"
                    item-text="text"
                    label="مجموعه"
                    @change="getSubComplexes"
                    solo
                  ></v-select>
                </v-col>

                <v-col cols="4">
                  <v-select
                    v-model="selectedSubComplexId"
                    :items="subComplexes"
                    item-value="id"
                    item-text="text"
                    label="زیر مجموعه"
                    @change="getUsers"
                    solo
                  ></v-select>
                </v-col>
                <v-col cols="4">
                  <v-select
                    v-model="support.userId"
                    :items="users"
                    item-value="id"
                    item-text="text"
                    label="کاربر"
                    solo
                  ></v-select>
                </v-col>
              </v-row>
              <v-row>
                <v-col cols="12" sm="12" md="4">
                  <h3>شروع :</h3>
                  <v-time-picker
                    v-model="support.startTime"
                    elevation="15"
                    class="mt-4"
                    format="24hr"
                    scrollable
                    min="00:01"
                    max="23:59"
                  ></v-time-picker>
                </v-col>

                <v-col cols="12" sm="12" md="4">
                  <h3>پایان :</h3>
                  <v-time-picker
                    label="sdf"
                    elevation="15"
                    v-model="support.endTime"
                    class="mt-4"
                    format="24hr"
                    scrollable
                    min="00:01"
                    max="23:59"
                  ></v-time-picker>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">بستن</v-btn>
          <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
            >ذخیره</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";

export default Vue.extend({
  name: "AddNewSupport",
  components: {},
  data: () => ({
    isUserEdit: false,
    isComplexEdit: false,
    complexNameInput: "",
    userNameInput: "",
    supportId: null,
    selectedComplex: {},
    selectedUser: {},
    complexes: [],
    subComplexes: [],
    selectedComplexId: null,
    selectedSubComplexId: null,
    users: [],
    dialog: false,
    valid: true,
    loading: false,
    imageUrl: "",
    isUnit: false,
    support: {
      startTime: "",
      endTime: "",
      userId: "",
      complexId: "",
    },
  }),
  watch: {
    dialog: {
      handler() {
        if (!this.dialog) {
          this.support.startTime = null;
          this.support.endTime = null;
          this.support.userId = null;
          this.support.complexId = null;
          this.selectedSubComplexId = null;
          this.selectedComplexId = null;

          this.supportId = null;
        }
        if (this.supportId) {
          this.getSupport(this.supportId);
        }
      },
      deep: true,
    },
  },
  created() {
    this.getComplexes();
  },

  methods: {
    complexFilter(item, queryText, itemText) {
      const searchText = queryText.toLowerCase();
    },
    userFilter(item, queryText, itemText) {
      const searchText = queryText.toLowerCase();
    },
    async getComplexes() {
      await request.get(`/publicData/main-complexes`).then((response) => {
        var data = response.data.result;
        this.complexes = data;
      });
    },
    async getSubComplexes() {
      if (this.selectedComplexId) {
        await request
          .get(`/publicData/sub-complexes/${this.selectedComplexId}`)
          .then((response) => {
            var data = response.data.result;
            this.subComplexes = data;
            this.selectedSubComplexId = this.selectedComplexId;
            this.getUsers();
            this.subComplexes.unshift({ id: null, text: "انتخاب..." });
          });
      }
    },
    async getUsers() {
      if (this.selectedSubComplexId) {
        await request
          .get(`/publicData/main-complexes-users/${this.selectedSubComplexId}`)
          .then((response) => {
            var data = response.data.result;
            this.users = data;
            this.users.unshift({ id: null, text: "انتخاب..." });
          });
      }
    },
    async getSupport(id) {
      await request.get(`/support/${id}`).then((response) => {
        var data = response.data.result;
        this.support.startTime = data.startTime;
        this.support.endTime = data.endTime;
        this.support.userId = data.userId;
        this.support.complexId = data.complexId;
        this.selectedSubComplexId = data.subComplexId;
        this.selectedComplexId = data.complexId;
        this.complexes = this.getComplexes();
        this.subComplexes = [
          { id: data.subComplexId, text: data.subComplexName },
        ];
        this.users = [{ id: data.userId, text: data.userName }];
        this.selectedUser = { id: data.userId, text: data.userName };
        this.selectedComplex = { id: data.complexId, text: data.complexName };
        this.selectedSubComplex = {
          id: data.subComplexId,
          text: data.subComplexName,
        };
        this.isComplexEdit = true;
        this.isUserEdit = true;
      });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;
      this.support.complexId = this.selectedComplexId;
      if (this.supportId) {
        request
          .put(`/support/${this.supportId}`, this.support)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadSupportes");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      } else {
        request
          .post("/support", this.support)
          .then((response) => {
            this.dialog = false;
            this.$emit("reloadSupportes");
            // this.$snotify.success("کابر با موفقیت با موفقیت ثبت گردید");
          })
          .finally(() => {
            this.loading = false;
          });
      }
    },
  },
});
</script>

<style scoped>
.v-card--reveal {
  align-items: center;
  bottom: 0;
  justify-content: center;
  opacity: 0.5;
  position: absolute;
  width: 100%;
}

.card-form-img {
  padding: 0px !important;
}

.icon-btn-modal {
  position: absolute;
  font-size: 18px !important;
  color: #fff !important;
  padding: 8px;
  border-radius: 50%;
}

.icon-btn-modal:hover {
  cursor: pointer;
}

.icon-btn-upload {
  position: absolute;
  left: 60%;
  bottom: 33%;
  color: #fff !important;
  /*padding: 8px;*/
  border-radius: 50%;
  /*background: #35495E !important;*/
  text-align: center;
  display: flex;
  margin: auto;
  justify-content: center;
  align-items: center;
  /*height: 20px !important;*/
  /*width: 20px !important;*/
}

.v-icon {
  color: #fff !important;
  font-size: 18px !important;
  text-align: center;
  background: #35495e !important;
}

.logo-title {
  text-align: center;
  display: flex;
  justify-content: center;
  margin-bottom: 15px;
}
</style>
