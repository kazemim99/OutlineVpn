<template>
  <div>
    <v-row class="mb-4">
      <Breadcrump class="mb-2" :crumbs="crumbs" />
      <v-spacer></v-spacer>

      <v-btn
        @click="$router.go(-1)"
        class="mx-10 mt-5"
        fab
        small
        dark
        color="indigo"
      >
        <v-icon dark> mdi-arrow-left </v-icon>
      </v-btn>
    </v-row>
    <v-card>
      <v-card-title>
        <span class="text-h5">کدهای فعالیت</span>
      </v-card-title>
      <v-card-text>
        <v-form ref="form" v-model="valid" lazy-validation>
          <v-container>
            <h3 style="margin-bottom: 5px">صفحه ورودی</h3>
            <v-row>
              <v-col cols="12" sm="12" md="12">
                <v-text-field
                  outlined
                  clearable
                  type="number"
                  v-model="activity.loginPass"
                  label="رمز ورود دستگاه  *"
                  placeholder=" "
                  autocomplete="false"
                  :rules="loginPassRules"
                  required
                ></v-text-field>
              </v-col>
            </v-row>
            <v-divider style="margin: 10px"></v-divider>
            <h3 style="margin-bottom: 5px">کد عدم شارژدهی قطعی</h3>

            <v-row>
              <v-col cols="4" sm="12" md="4">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.lockersDeactivePass"
                  label="رمز فعالسازی لاکر *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="4" sm="12" md="4">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.lockersActivePass"
                  label="رمز غیر فعالسازی لاکر *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="4" sm="12" md="4">
                <v-switch
                  v-model="activity.lockersBlockActive"
                  :label="`وضعیت: ${
                    activity.lockersBlockActive ? 'فعال' : 'غیر فعال'
                  }`"
                ></v-switch>
              </v-col>
            </v-row>

            <v-divider style="margin: 10px"></v-divider>
            <v-spacer vertical></v-spacer>
            <h3 style="margin-bottom: 5px">کد عدم شارژدهی ثابت</h3>

            <v-row>
              <v-col cols="3" sm="12" md="3">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.fixedTimeLockersActivePass"
                  label="رمز فعالسازی لاکر *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="3" sm="12" md="3">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.fixedTimeLockersDeactivePass"
                  label="رمز فعالسازی لاکر *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="3" sm="12" md="3">
                <v-switch
                  v-model="activity.fixedTimeLockersBlockActive"
                  :label="`وضعیت: ${
                    activity.fixedTimeLockersBlockActive ? 'فعال' : 'غیر فعال'
                  }`"
                ></v-switch>
              </v-col>
              <div>
                <v-row>
                  <v-col cols="12" sm="12" md="6">
                    <h3>شروع :</h3>
                    <v-time-picker
                      v-model="activity.fixedTimeLockersBlockStartTime"
                      elevation="15"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>

                  <v-col cols="12" sm="12" md="6">
                    <h3>زمان پایان :</h3>
                    <v-time-picker
                      label="sdf"
                      elevation="15"
                      v-model="activity.fixedTimeLockersBlockEndTime"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>
                </v-row>
              </div>
            </v-row>
            <br />
            <br />
            <v-divider style="margin: 10px"></v-divider>
            <v-spacer vertical></v-spacer>
            <h3 style="margin-bottom: 5px">کد اعلام ساعت پایانی</h3>
            <v-row>
              <v-col cols="3" sm="12" md="3">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.userInputTimeWarningActivePass"
                  label="رمز فعالسازی  *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="3" sm="12" md="3">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.userInputTimeWarningdeactivePass"
                  label="رمز غیرفعالسازی *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="3" sm="12" md="3">
                <v-switch
                  v-model="activity.userInputTimeWarningActive"
                  :label="`وضعیت: ${
                    activity.userInputTimeWarningActive ? 'فعال' : 'غیر فعال'
                  }`"
                ></v-switch>
              </v-col>
              <div>
                <v-row>
                  <v-col cols="12" sm="12" md="12">
                    <h3>زمان اخطار :</h3>
                    <v-time-picker
                      v-model="activity.userTimeForWarning"
                      elevation="15"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>
                </v-row>
              </div>
            </v-row>

            <br />
            <br />
            <v-divider style="margin: 10px"></v-divider>
            <v-spacer vertical></v-spacer>
            <h3 style="margin-bottom: 5px">کد اعلام ساعت پایانی ثابت</h3>
            <v-row>
              <v-col cols="3" sm="12" md="3">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.fixedTimeWarningActivePass"
                  label="رمز فعالسازی  *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="3" sm="12" md="3">
                <v-text-field
                  outlined
                  clearable
                  v-model="activity.fixedTimeWarningDeactivePass"
                  label="رمز غیرفعالسازی *"
                  placeholder=" "
                  autocomplete="false"
                  required
                ></v-text-field>
              </v-col>
              <v-col cols="3" sm="12" md="3">
                <v-switch
                  v-model="activity.fixedTimeWarningActive"
                  :label="`وضعیت: ${
                    activity.fixedTimeWarningActive ? 'فعال' : 'غیر فعال'
                  }`"
                ></v-switch>
              </v-col>
              <div>
                <v-row>
                  <v-col cols="12" sm="12" md="4">
                    <h3>زمان اخطار :</h3>
                    <v-time-picker
                      v-model="activity.fixedWarningTime"
                      elevation="15"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>
                  <v-col cols="12" sm="12" md="4">
                    <h3>زمان شروع اخطار :</h3>
                    <v-time-picker
                      v-model="activity.fixedTimeStartWarn"
                      elevation="15"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>
                  <v-col cols="12" sm="12" md="4">
                    <h3>زمان پایان اخطار :</h3>
                    <v-time-picker
                      v-model="activity.fixedTimeEndWarn"
                      elevation="15"
                      class="mt-4"
                      format="24hr"
                      scrollable
                      min="00:01"
                      max="23:59"
                    ></v-time-picker>
                  </v-col>
                </v-row>
              </div>
            </v-row>
          </v-container>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn
          v-can="'CodeActivity_Edit'"
          :loading="loading"
          color="blue darken-1"
          text
          @click="submit()"
          >ذخیره</v-btn
        >
      </v-card-actions>
    </v-card>
  </div>
</template>
<script>
import request from "@/utils/request";
import Vue from "vue";
import Breadcrump from "@/components/common/Breadcrump.vue";

export default {
  name: "CodeActivity",
  components: {
    Breadcrump,
  },
  data() {
    return {
      crumbs: [
        {
          text: "خانه",
          disabled: false,
          href: "/",
        },
        {
          text: "مجموعه ها",
          disabled: false,
          href: "/complexes",
        },
        {
          text: `مجموعه ${this.$store.state.selectedComplexName}`,
          disabled: false,
          href: `/complex-units/${this.$route.params.id}`,
        },
        {
          text: "دستگاهها",
          href: `/complex-devices/${this.$store.state.selectedComplexId}`,
        },
        {
          text: `کدهای فعالیت ${this.$store.state.selectedDeviceName}`,
          disabled: true,
        },
      ],
      deviceId: null,
      activity: null,
    };
  },
  LoginPassRules: [(v) => !!v || "لطفا رمز ورود دستگاه را وارد نمایید"],

  created() {
    this.deviceId = this.$route.params.id;
    this.getActivity();
  },
  methods: {
    getActivity() {
      request
        .get(`/deviceManagement/activites/${this.deviceId}`)
        .then((response) => {
          this.activity = response.data.result;
        });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      request
        .put(`/deviceManagement/activites/${this.deviceId}`, this.activity)
        .then(() => {
          Vue.swal("", "کدهای فعالیت  با موفقیت ویرایش شد", "success");
        });
    },
  },
};
</script>
