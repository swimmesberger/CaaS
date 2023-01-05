import { INavData } from '@coreui/angular';


export const navItems: INavData[] = [
  {
    title: true,
    name: 'Management'
  },
  {
    name: 'Shops',
    url: './dashboard',
    iconComponent: { name: 'cil-view-module' }
  },
  {
    name: 'Add shop',
    url: './dashboard',
    iconComponent: { name: 'cil-plus' }
  }
];
