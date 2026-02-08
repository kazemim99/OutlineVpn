<template>
  <div>
    <v-card>
      <v-card-title>پشتیبانی</v-card-title>
      <v-card-text>
        <v-textarea v-model="message" label="متن پیام"></v-textarea>

        <v-file-input v-model="attachment" label="فایل ضمیمه"></v-file-input>
        <v-btn @click="sendMessage" color="primary">ارسال</v-btn>
      </v-card-text>
    </v-card>
    <v-card>
      <v-card-title>لیست پیامهای قبلی</v-card-title>
      <v-card-text>
        <v-list>
          <v-list-item v-for="(message, index) in messageHistory" :key="index">
            <v-list-item-content>
              <v-list-item-title>{{ message.sender }}</v-list-item-title>
              <v-list-item-subtitle>{{
                message.timestamp
              }}</v-list-item-subtitle>
              <v-list-item-text>{{ message.text }}</v-list-item-text>
            </v-list-item-content>
          </v-list-item>
        </v-list>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import Vue from "vue";
import request from "@/utils/request";
export default {
  data() {
    return {
      model: {
        message: "",
        attachment: null,
      },
      messageHistory: [],
    };
  },
  mounted() {
    this.tickets();
  },
  methods: {
    async tickets() {
      await request.get(`/user-tickets/`).then((response) => {
        const data = response.data.result;
        this.messageHistory = data.result;
      });
    },
    async sendMessage() {
      if (!this.$refs.form.validate()) {
        return;
      }
      this.loading = true;

      const form_data = new FormData();

      for (const key in this.model) {
        form_data.append(key, this.model[key]);
      }
      request.defaults.headers.common.accept = "multipart/form-data";

      request
        .post(`/send-ticket`, form_data)
        .then((response) => {
          Vue.swal("", "پیام شما با موفقیت ارسال شد", "success");
          (this.model.attachment = ""), (this.model.message = "");
        })
        .finally(() => {
          this.loading = false;
        });
      // Use your API to send the message and attachment to the server
      // and retrieve the updated message history.
      // const updatedHistory = await api.sendMessage(this.message, this.attachment)
      // this.messageHistory = updatedHistory
    },
  },
};
</script>
