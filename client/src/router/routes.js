
const routes = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', component: () => import('pages/Index.vue') },
      { path: '/departments', component: () => import('pages/Departments.vue') },
      { path: '/departments/:id', component: () => import('pages/EditDepartment.vue') },
      { path: '/course_info', component: () => import('pages/CourseInfo.vue') },
      { path: '/course_info/:id', component: () => import('pages/EditCourseInfo.vue') },
      { path: '/course_info/:courseInfoId/courses/:courseId', component: () => import('pages/EditCourse.vue') },
      { path: '/course_info/:courseInfoId/courses/:courseId/assignments/:assignmentId', component: () => import('pages/EditAssignment.vue') },
      { path: '/import_excel', component: () => import('pages/ImportExcel.vue') }
    ]
  }
]

// Always leave this as last one
if (process.env.MODE !== 'ssr') {
  routes.push({
    path: '*',
    component: () => import('pages/Error404.vue')
  })
}

export default routes
