import { INavData } from '@coreui/angular';


export const navItems: INavData[] = [
  {
    title: true,
    name: 'Management'
  },
  {
    name: 'Dashboard',
    url: '/dashboard',
    iconComponent: { name: 'cil-speedometer' }
  }
];
