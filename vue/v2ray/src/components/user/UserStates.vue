
<template>
  <v-container fluid>
    <v-row>
      <v-col cols="6" sm="6">
        <v-select
          :items="userStates"
          v-on:change="selectState"
          v-model="userState"
          item-value="id"
          item-text="text"
          label="وضعیت"
          solo
        ></v-select>
      </v-col>
    </v-row>
  </v-container>
</template>
 <script>
import Vue from "vue";

export default Vue.extend({
  name: "UserStates",
  data: () => ({
    userState: 0,
  }),

  created() {
    this.getUserStates();
  },
  computed: {
    userStates() {
      return this.$store.state.userStates;
    },
  },
  methods: {
    selectState() {
      this.$emit("SelectedState", this.userState);
    },
    getUserStates() {
      this.$store.dispatch("getUserStates");
    },
  },
});
</script>
 