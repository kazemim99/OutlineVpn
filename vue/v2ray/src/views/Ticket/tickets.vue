<template>
    <v-container>
      <v-card>
        <v-card-title>Support Ticket History</v-card-title>
        <v-data-table
          :headers="headers"
          :items="ticketHistory"
          :items-per-page="itemsPerPage"
          :page.sync="page"
          hide-default-footer
        >
          <template v-slot:item.number="{ item }">
            <router-link :to="{ name: 'ticket', params: { number: item.number } }">{{ item.number }}</router-link>
          </template>
        </v-data-table>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-pagination
            v-model="page"
            :length="Math.ceil(ticketHistory.length / itemsPerPage)"
            circle
          ></v-pagination>
        </v-card-actions>
      </v-card>
    </v-container>
  </template>
  
  <script>
  export default {
    data() {
      return {
        headers: [
          { text: 'Ticket Number', value: 'number' },
          { text: 'Subject', value: 'subject' },
          { text: 'Status', value: 'status' },
          { text: 'Created At', value: 'createdAt' }
        ],
        ticketHistory: [],
        page: 1,
        itemsPerPage: 10
      }
    },
    async mounted() {
      // Use your API to retrieve the ticket history and store it in the component's data.
    //   this.ticketHistory = await api.getTicketHistory()
    }
  }
  </script>
  