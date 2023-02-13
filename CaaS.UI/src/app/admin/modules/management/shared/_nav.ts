import { INavData } from '@coreui/angular';


export const navItems: INavData[] = [
  {
    title: true,
    name: 'Management'
  },
  {
    name: 'Shops',
    url: '/admin/shops',
    iconComponent: { name: 'cil-view-module' }
  },
  {
    name: 'Add shop',
    url: '/admin/shop',
    iconComponent: { name: 'cil-plus' }
  }
];
