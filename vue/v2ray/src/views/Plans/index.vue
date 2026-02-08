<template>
  <v-container fluid>
    <v-row dense>
      <v-col v-for="card in cards" :key="card.id" :cols="card.flex">
        <v-card :loading="card.loading" class="mx-auto my-12" max-width="374">
          <template slot="progress">
            <v-progress-linear
              color="deep-purple"
              height="10"
              indeterminate
            ></v-progress-linear>
          </template>

          <v-img height="250" :src="card.image"></v-img>

          <v-card-title v-text="card.title"></v-card-title>

          <v-divider class="mx-4"></v-divider>

          <v-card-title
            v-text="`${card.period / 30} ماهه ${card.trafficCapacity} گیگابایت`"
          ></v-card-title>
          <v-card-title
            v-text="`${formatPrice(card.price)} تومان `"
          ></v-card-title>

          <v-card-actions>
            <v-btn color="deep-purple lighten-2" text @click="reserve(card.id)">
              خرید
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import request from "@/utils/request";
import Vue from "vue";

export default {
  data: () => ({
    cards: [],
  }),
  mounted() {
    this.getConsumedTraffic();
  },

  methods: {
    formatPrice(value) {
      const val = (value / 1).toFixed().replace(".", ",");
      return val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    },
    getConsumedTraffic() {
      request.get(`plan/plans`).then((response) => {
        const data = response.data.result;
        this.cards = data.result;
      });
    },
    reserve(id) {
      this.$router.push(`/checkout/${id}`);
    },
  },
};

//  createKey(id){
//     Vue.swal({
//       title: "ایا مطمئن  هستید",
//       icon: "warning",
//       showCancelButton: true,
//       confirmButtonColor: "#3085d6",
//       cancelButtonColor: "#d33",
//       confirmButtonText: "بله ,حذف شود",
//       cancelButtonText: "انصراف",
//     }).then((result) => {
//       if (result.isConfirmed) {
//         request
//           .delete(`/support/${id}`)
//           .then(() => {
//             Vue.swal("", "پشتیبان با موفقیت حذف گردید", "success");
//             this.getSupportes();
//           })
//           .finally(() => {
//             this.uploadLoading = false;
//           });
//       }
//     });
//   },
</script>
