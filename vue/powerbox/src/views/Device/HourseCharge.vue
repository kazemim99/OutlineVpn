<template>
  <v-form ref="form" v-model="valid" lazy-validation>
    <v-container>
      <deviceFilterDropDown ref="complex" />
      <calenderSelect ref="dateRef" />

      <v-row>
        <v-col cols="2">
          <v-select
            v-model="filter.cableType"
            :items="cableTypes"
            multiple
            item-value="id"
            item-text="text"
            label="نوع کابل"
          ></v-select>
        </v-col>
        <v-col cols="2">
          <v-select
            v-model="filter.rate"
            :items="getRates()"
            multiple
            item-value="id"
            item-text="text"
            label="شماره امتیاز"
          ></v-select>
        </v-col>
      </v-row>
      <v-spacer></v-spacer>
      <v-btn :loading="loading" color="blue darken-1" text @click="submit()"
        >ارسال</v-btn
      >

      <br />
      <br />
      <v-row>
        <v-col cols="6">تعداد دفعات استفاده شده</v-col>
        <v-col cols="6">تعداد ساعات استفاده شده</v-col>
      </v-row>
      <v-row>
        <v-col cols="6">
          <v-col cols="3">یک روز اخیر</v-col>
          <v-col cols="2" style="color: red">{{ dayuserCount }} نفر</v-col>
        </v-col>
        <v-col cols="6">
          <v-col cols="3">یک روز اخیر</v-col>
          <v-col cols="2" style="color: red">{{ dayhourseSum }} ساعت</v-col>
        </v-col>

        <v-col cols="6">
          <v-col cols="3">یک هفته اخیر</v-col>
          <v-col cols="2" style="color: red">{{ weekuserCount }} نفر</v-col>
        </v-col>
        <v-col cols="6">
          <v-col cols="3">یک هفته اخیر</v-col>
          <v-col cols="2" style="color: red">{{ weekhourseSum }} ساعت</v-col>
        </v-col>
        <v-col cols="6">
          <v-col cols="3">یک ماه اخیر</v-col>
          <v-col cols="2" style="color: red">{{ monthuserCount }} نفر</v-col>
        </v-col>
        <v-col cols="6">
          <v-col cols="3">یک ماه اخیر</v-col>
          <v-col cols="2" style="color: red">{{ monthhourseSum }} ساعت</v-col>
        </v-col>

        <v-col cols="6">
          <v-col cols="3">تعداد کل کابران</v-col>
          <v-col cols="2" style="color: red">{{ userCount }} نفر</v-col>
        </v-col>
        <v-col cols="6">
          <v-col cols="3">مجموع کل ساعات شارژ</v-col>
          <v-col cols="2" style="color: red">{{ hourseSum }} ساعت</v-col>
        </v-col>
      </v-row>
    </v-container>
  </v-form>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";
import deviceFilterDropDown from "@/components/common/DeviceFilterDropDown.vue";
import calenderSelect from "@/components/common/CalenderSelect.vue";
import qs from "qs";

export default Vue.extend({
  name: "HourseCharge",
  components: { deviceFilterDropDown, calenderSelect },
  data: () => ({
    cableTypes: [
      {
        text: "Apple",
        id: "Apple",
      },
      {
        text: "Android",
        id: "Android",
      },
    ],
    hourseSum: 0,
    userCount: 0,

    dayhourseSum: 0,
    dayuserCount: 0,

    weekhourseSum: 0,
    weekuserCount: 0,

    monthhourseSum: 0,
    monthuserCount: 0,
    valid: true,
    loading: false,
    filter: {
      from: "",
      to: "",
      deviceId: null,
      complexId: null,
      rate: [],
      cableType: [],
    },
  }),

  methods: {
    getRates() {
      let rates = [];
      for (let i = 1; i <= 6; i++) {
        rates.push({
          id: parseInt(i),
          text: `${i}`,
        });
      }
      return rates;
    },
    submit() {
      if (!this.$refs.form.validate()) {
        return;
      }
      const filterQuery = Object.keys(this.filter)
        .filter((x) => this.filter[x] !== null && this.filter[x] !== undefined)
        .map((key) => `${key}=${this.filter[key]}`)
        .join("&");
      this.loading = true;
      request
        .get(`/deviceManagement/hourse-charge-user`, {
          params: {
            cableType: this.filter.cableType,
            rate: this.filter.rate,
            from: this.$refs.dateRef.from,
            to: this.$refs.dateRef.to,
            complexId: this.$refs.complex.selectedSubComplexId
              ? this.$refs.complex.selectedSubComplexId
              : this.$refs.complex.selectedComplexId,
            deviceId: this.$refs.complex.deviceId,
          },
          paramsSerializer: (params) => {
            return qs.stringify(params, { arrayFormat: "repeat" });
          },
        })
        .then((response) => {
          this.loading = false;
          var data = response.data.result;
          console.log(data);
          (this.hourseSum = data.hourse), 
          (this.userCount = data.userCount);
          (this.dayhourseSum = data.dayhourseSum),
            (this.dayuserCount = data.dayuserCount);
          (this.weekhourseSum = data.weekhourseSum),
            (this.weekuserCount = data.weekuserCount);
          (this.monthhourseSum = data.monthhourseSum),
            (this.monthuserCount = data.monthuserCount);
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
