import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin.service';
import { AdminOverview } from '../../models/types';

@Component({ selector: 'app-admin-dashboard', imports: [NgIf, NgFor, FormsModule, RouterLink, CurrencyPipe, DatePipe], templateUrl: './admin-dashboard.html', styleUrl: './admin-dashboard.css' })
export class AdminDashboardComponent implements OnInit {
  private service = inject(AdminService);
  data = signal<AdminOverview | null>(null); loading = signal(true); error = signal(''); activeTab = signal<'review' | 'users' | 'categories'>('review'); actionId = signal<string | null>(null); newCategoryName = ''; addingCategory = false;
  ngOnInit(): void { this.load(); }
  load(): void { this.loading.set(true); this.service.overview().subscribe({ next: r => { this.loading.set(false); r.success && r.data ? this.data.set(r.data) : this.error.set(r.message || 'Unable to load admin overview.'); }, error: e => { this.loading.set(false); this.error.set(e.error?.message || 'Unable to load admin overview.'); } }); }
  statusClass(status: string): string { return `badge-${status.toLowerCase()}`; }
  review(id: string, action: 'publish' | 'reject'): void { this.actionId.set(id); const request = action === 'publish' ? this.service.publishEvent(id) : this.service.rejectEvent(id); request.subscribe({ next: r => { this.actionId.set(null); r.success ? this.load() : this.error.set(r.message); }, error: e => { this.actionId.set(null); this.error.set(e.error?.message || 'Action failed.'); } }); }
  addCategory(): void {
    const name = this.newCategoryName.trim();
    if (!name || this.addingCategory) return;
    this.addingCategory = true; this.error.set('');
    this.service.createCategory(name).subscribe({
      next: r => { this.addingCategory = false; if (r.success) { this.newCategoryName = ''; this.load(); } else this.error.set(r.message || 'Unable to create category.'); },
      error: e => { this.addingCategory = false; this.error.set(e.error?.message || 'Unable to create category.'); }
    });
  }
  deleteCategory(id: string): void { if (!confirm('Delete this category?')) return; this.service.deleteCategory(id).subscribe({ next: r => r.success ? this.load() : this.error.set(r.message), error: e => this.error.set(e.error?.message || 'Unable to delete category.') }); }
}
