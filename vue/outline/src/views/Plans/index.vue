<template>
  <v-container fluid>
    <v-row dense>
      <v-col v-for="card in cards" :key="card.title" :cols="card.flex">
        <v-card :loading="card.loading" class="mx-auto my-12" max-width="374">
          <template slot="progress">
            <v-progress-linear
              color="deep-purple"
              height="10"
              indeterminate
            ></v-progress-linear>
          </template>

          <v-img
            height="250"
            src="https://cdn.vuetifyjs.com/images/cards/cooking.png"
          ></v-img>

          <v-card-title v-text="card.title"></v-card-title>

          <v-divider class="mx-4"></v-divider>

          <v-card-title></v-card-title>

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

export default {
  data: () => ({
    input: {
      phoneNumber: "09123135143",
    },
    selection: 1,
    cards: [
      {
        id: 1,
        title: "10 گیگ یک ماه",
        src: "https://cdn.vuetifyjs.com/images/cards/house.jpg",
        flex: 4,
        loading: false,
      },
      {
        id: 2,
        title: "20 گیگ یک ماه",
        src: "https://cdn.vuetifyjs.com/images/cards/road.jpg",
        flex: 4,
        loading: false,
      },
      {
        id: 3,
        title: "30 گیگ یک ماه",
        src: "https://cdn.vuetifyjs.com/images/cards/plane.jpg",
        flex: 4,
        loading: false,
      },
    ],
  }),

  methods: {
    reserve(id) {
      request
        .post(`/keys`,this.input)
        .then(() => {
          alert("success")
        })
        .finally(() => {
          console.log("finaly")
        });
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


