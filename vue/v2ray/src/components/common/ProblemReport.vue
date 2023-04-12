<template>
  <v-row justify="center">
    <v-dialog v-model="dialog" persistent max-width="600px">
      <template v-slot:activator="{ on, attrs }">
        <v-btn
          color="error"
          dark
          v-bind="attrs"
          v-on="on"
          style="margin-bottom: 20px"
        >
          گزارش قطعی و پیام
        </v-btn>
      </template>
      <v-card>
        <v-card-title>
          <span class="text-h5">گزارش قطعی یا پیام</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form" v-model="valid" lazy-validation>
            <v-container>
              <v-row>
                <v-col cols="6" sm="6">
                  <v-select
                    :items="osList"
                    :rules="select1"
                    v-on:change="selectOs"
                    v-model="os"
                    required
                    item-value="id"
                    item-text="text"
                    label="سیستم عامل"
                  ></v-select>
                </v-col>
                <v-col cols="6" sm="6">
                  <v-select
                    :items="oprations"
                    :rules="select2"
                    v-on:change="selectOperator"
                    v-model="operator"
                    required
                    item-value="id"
                    item-text="text"
                    label="اپراتور"
                  ></v-select>
                </v-col>
                <v-col cols="6" sm="6">
                  <v-textarea
                    outlined
                    v-model="model.despriction"
                    name="input-7-4"
                    label="توضیحات"
                    placeholder="با دادن توضیحات بیشتر ما را در رفع هرچه بهتر مشکل یاری کنید"
                  ></v-textarea>
                </v-col>

                <v-col cols="6" sm="6">
                  <v-switch
                    v-model="model.returnMony"
                    :label="`بازگشت وجه؟ : ${model.returnMony ? 'بله' : 'خیر'}`"
                  ></v-switch>
                </v-col>
              </v-row>
            </v-container>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">
            بستن
          </v-btn>
          <v-btn
            :loading="loading"
            color="blue darken-1"
            text
            @click="submit()"
          >
            ذخیره
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>
<script>
import request from "@/utils/request";
import Vue from "vue";

export default {
  data: () => ({
    dialog: false,
    dialogLogo: false,
    valid: true,
    oprations: [],
    operator: {},
    osList: [],
    os: {},
    select1: [(v) => !!v || "ُسیستم عامل را وارد نمایید"],
    select2: [(v) => !!v || "اپراتور را وارد نمایید"],
    loading: false,
    model: {
      operator: 0,
      oS: 0,
      despriction: "",
    },
  }),

  created() {
    this.getOs();
    this.getOperations();
  },
  methods: {
    selectOperator() {
      this.model.operator = this.operator;
    },
    selectOs() {
      this.model.oS = this.os;
    },
    async getOs() {
      await request.get(`/publicData/get-operations`).then((response) => {
        this.oprations = response.data.result;
      });
    },
    async getOperations() {
      await request.get(`/publicData/get-os`).then((response) => {
        this.osList = response.data.result;
      });
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;
      request
        .post("/problemReport", this.model)
        .then((response) => {
          this.dialog = false;
          this.$emit("reloadUsers");
          Vue.swal("", "گزارش شما ارسال و بزودی بررسی خواهد شد", "success");
        })
        .catch((e) => {
          console.log(e);
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>