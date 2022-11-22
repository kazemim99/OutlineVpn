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
      <template  v-slot:activator="{ on, attrs }">
        <v-btn color="primary" dark v-bind="attrs" v-on="on"
          >گزارش </v-btn
        >
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">گزارش حضور و غیاب</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    outlined
                    clearable
                    v-model="attendance.fullName"
                    label="نام و نام خانوادگی  *"
                    placeholder=" "
                    autocomplete="false"
                    :rules="fullNameRules"
                    required
                  ></v-text-field>
                </v-col>

                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    outlined
                    clearable
                    v-model="attendance.fromDate"
                    label="از تاریخ *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>
                <v-col cols="4" sm="12" md="4">
                  <v-text-field
                    outlined
                    clearable
                    v-model="attendance.toDate"
                    label="تا تاریخ *"
                    placeholder=" "
                    autocomplete="false"
                    required
                  ></v-text-field>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
            >ارسال</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";

export default Vue.extend({
  name: "AttendanceReport",
  components: {},
  data: () => ({
    dialog: false,
    valid: false,
    loading: false,
    attendance: {
     
    },
    fullNameRules: [(v) => !!v || "لطفا نام را وارد نمایید"],
  }),
 

  methods: {
 
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

    
        request
          .post("/attendance/report", this.attendance)
          .then(() => {
            this.dialog = false;
          })
          .finally(() => {
            this.loading = false;
          });
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
