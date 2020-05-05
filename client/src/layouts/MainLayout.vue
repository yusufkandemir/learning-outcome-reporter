<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          icon="mdi-menu"
          aria-label="Menu"
          @click="leftDrawerMini = !leftDrawerMini"
        />

        <q-toolbar-title>
          <q-avatar>
            <img src="https://cdn.quasar.dev/logo/svg/quasar-logo.svg" />
          </q-avatar>Outcome Reporter
        </q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="leftDrawerOpen"
      :mini="!leftDrawerOpen || leftDrawerMini"
      :width="250"
      elevated
      persistent
    >
      <q-scroll-area class="fit">
        <q-list padding>
          <q-item to="/" exact clickable v-ripple>
            <q-item-section avatar>
              <q-icon name="mdi-view-dashboard"></q-icon>
            </q-item-section>

            <q-item-section>Dashboard</q-item-section>
          </q-item>

          <q-item to="/departments" clickable v-ripple>
            <q-item-section avatar>
              <q-icon name="mdi-domain"></q-icon>
            </q-item-section>

            <q-item-section>Departments</q-item-section>
          </q-item>
        </q-list>
      </q-scroll-area>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>

    <q-footer elevated class="bg-black text-center q-py-xs">
      <div>IKU Department of Computer Engineering &copy; 2020</div>
      <div>Made with 💖 by YODA</div>
    </q-footer>
  </q-layout>
</template>

<script>
export default {
  name: 'MainLayout',

  data () {
    return {
      leftDrawerMini: true,
      leftDrawerOpen: true
    }
  }
}
</script>
