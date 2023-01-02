import { Component, OnInit } from '@angular/core';
import {Observable} from "rxjs";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-page-header',
  templateUrl: './page-header.component.html',
  styleUrls: ['./page-header.component.scss'],
  host: {
    class: 'page-title-overlap bg-dark pt-4'
  }
})
export class PageHeaderComponent implements OnInit {
  protected $pageTitle: Observable<string | undefined>;

  constructor(private activatedRoute: ActivatedRoute) {
    this.$pageTitle = activatedRoute.title;
  }

  ngOnInit(): void {
  }

}
