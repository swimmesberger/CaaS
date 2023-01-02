import {Component} from '@angular/core';
import {ActivatedRoute, NavigationEnd, Router} from "@angular/router";
import {concat, filter, map, Observable, of, shareReplay} from "rxjs";

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent {
  protected $breadcrumbs: Observable<IBreadcrumbItem[]>;

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) {
    const initialNavigation = new NavigationEnd(0, "", "");
    this.$breadcrumbs = concat(of(initialNavigation), this.router.events).pipe(
        filter(e => e instanceof NavigationEnd),
        map(() => this.buildBreadcrumb()),
        shareReplay(1)
    );
  }

  private buildBreadcrumb(): IBreadcrumbItem[] {
    const breadcrumbs: IBreadcrumbItem[] = [];
    let currentRoute: ActivatedRoute | null = this.route.root;
    let url = '';
    do {
      const childrenRoutes: ActivatedRoute[] = currentRoute.children;
      currentRoute = null;
      for (let childRoute of childrenRoutes) {
        const routeSnapshot = childRoute.snapshot;
        url += '/' +  routeSnapshot.url.map((segment) => segment.path).join('/');
        breadcrumbs.push({
          label: childRoute.snapshot.routeConfig?.title?.toString() || '',
          url
        });
        currentRoute = childRoute;
      }
    } while (currentRoute);
    return breadcrumbs;
 }
}

interface IBreadcrumbItem {
  label: string;
  url?: string | any[];
}
