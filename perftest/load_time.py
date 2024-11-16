from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time
import json
import logging
from datetime import datetime
from typing import Dict, List
import sys
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
from pathlib import Path

class GameLoadTimeTest:
    def __init__(self, game_url: str, leaderboard_url: str):
        self.game_url = game_url
        self.leaderboard_url = leaderboard_url
        self.driver = None
        self.setup_logging()
        self.metrics = {
            'game': {
                'total_load_time': [],
                'asset_timings': [],
                'resource_sizes': [],
                'timestamp': []
            },
            'leaderboard': {
                'total_load_time': [],
                'asset_timings': [],
                'resource_sizes': [],
                'timestamp': []
            }
        }
        # Create reports directory if it doesn't exist
        self.reports_dir = Path('performance_reports')
        self.reports_dir.mkdir(exist_ok=True)

    def setup_logging(self):
        logging.basicConfig(
            level=logging.INFO,
            format='%(asctime)s - %(levelname)s - %(message)s',
            handlers=[
                logging.StreamHandler(sys.stdout)  # Log only to console
            ]
        )

    def start_browser(self):
        try:
            chrome_options = webdriver.ChromeOptions()
            # options.add_argument('--enable-precise-memory-info')
            # options.add_argument('--disable-cache')  # Disable browser cache
            # options.add_argument('--no-sandbox')
            # options.add_argument('--disable-dev-shm-usage')
            # options.add_experimental_option("prefs", {"profile.managed_default_content_settings.images": 2}) 
            # options.add_argument("--disable-setuid-sandbox") 
            #
            # options.add_argument("--remote-debugging-port=9222")
            #
            # options.add_argument("--disable-dev-shm-using") 
            # options.add_argument("--disable-extensions") 
            # options.add_argument("--disable-gpu") 
            # options.add_argument("start-maximized") 
            # options.add_argument("disable-infobars")
            # options.add_argument(r"user-data-dir=.\cookies\\test")
            
            # Enable performance logging
            # options.set_capability('goog:loggingPrefs', {'performance': 'ALL', 'browser': 'ALL'})
            chrome_options.add_argument('--headless=new')
            chrome_options.add_argument('--no-sandbox')
            chrome_options.add_argument('--disable-dev-shm-usage')
            chrome_options.add_argument('--disable-gpu')
    
            # Use specific path for ChromeDriver
            service = Service(executable_path='/usr/local/bin/chromedriver')
            self.driver = webdriver.Chrome(service=service, options=chrome_options)
            
            self.driver.maximize_window()
            self.wait = WebDriverWait(self.driver, 10)

            self.driver.set_page_load_timeout(30)
            logging.info("Browser started successfully")
            
        except Exception as e:
            logging.error(f"Error starting browser: {str(e)}")
            raise

    def safe_clear_storage(self):
        """Safely clear storage if available"""
        try:
            self.driver.execute_script("if (window.localStorage) { window.localStorage.clear(); }")
            self.driver.execute_script("if (window.sessionStorage) { window.sessionStorage.clear(); }")
        except Exception:
            pass  # Ignore if storage is not accessible


    def measure_page_load(self, url: str, page_type: str) -> Dict:
        """Measure load time and collect performance metrics for a specific page"""
        try:
            logging.info(f"Measuring {page_type} page load time for: {url}")
            
            start_time = time.time()
            self.driver.get(url)
            
            WebDriverWait(self.driver, 30).until(
                lambda d: d.execute_script("return document.readyState") == "complete"
            )

            navigation_timing = self.driver.execute_script("""
                let timing = performance.timing;
                try {
                    return {
                        'dns': timing.domainLookupEnd - timing.domainLookupStart,
                        'tcp': timing.connectEnd - timing.connectStart,
                        'request': timing.responseStart - timing.requestStart,
                        'response': timing.responseEnd - timing.responseStart,
                        'dom_processing': timing.domComplete - timing.domLoading,
                        'total': timing.loadEventEnd - timing.navigationStart
                    };
                } catch (e) {
                    return {
                        'dns': 0, 'tcp': 0, 'request': 0,
                        'response': 0, 'dom_processing': 0, 'total': 0
                    };
                }
            """)

            resource_timing = self.driver.execute_script("""
                try {
                    let resources = performance.getEntriesByType('resource');
                    return resources.map(resource => ({
                        name: resource.name,
                        type: resource.initiatorType,
                        duration: resource.duration,
                        size: resource.transferSize || 0,
                        startTime: resource.startTime
                    }));
                } catch (e) {
                    return [];
                }
            """)

            total_load_time = time.time() - start_time
            
            # Store metrics with timestamp
            current_time = datetime.now()
            self.metrics[page_type]['total_load_time'].append(total_load_time)
            self.metrics[page_type]['asset_timings'].append(navigation_timing)
            self.metrics[page_type]['resource_sizes'].append(resource_timing)
            self.metrics[page_type]['timestamp'].append(current_time)
            
            return {
                'total_load_time': total_load_time,
                'navigation_timing': navigation_timing,
                'resource_timing': resource_timing,
                'timestamp': current_time
            }
            
        except Exception as e:
            logging.error(f"Error measuring {page_type} page load: {str(e)}")
            return None

    def analyze_slow_assets(self, resource_timing: List[Dict], threshold_ms: float = 500) -> List[Dict]:
        """Identify assets that take longer than the threshold to load"""
        slow_assets = []
        if not resource_timing:
            return slow_assets
            
        for resource in resource_timing:
            if resource['duration'] > threshold_ms:
                slow_assets.append({
                    'name': resource['name'],
                    'type': resource['type'],
                    'duration': resource['duration'],
                    'size': resource['size']
                })
        return sorted(slow_assets, key=lambda x: x['duration'], reverse=True)

    def generate_optimization_suggestions(self, slow_assets: List[Dict]) -> List[str]:
        """Generate optimization suggestions based on slow assets"""
        suggestions = []
        
        for asset in slow_assets:
            if asset['type'] == 'script':
                suggestions.append(f"Consider deferring or async loading: {asset['name']}")
                if asset['size'] > 50000:  # 50KB
                    suggestions.append(f"Large script file ({asset['size']/1024:.1f}KB): Consider splitting or minifying {asset['name']}")
            
            elif asset['type'] == 'img':
                if asset['size'] > 200000:  # 200KB
                    suggestions.append(f"Large image ({asset['size']/1024:.1f}KB): Compress or use WebP format for {asset['name']}")
                suggestions.append(f"Implement lazy loading for image: {asset['name']}")
            
            elif asset['type'] == 'css':
                suggestions.append(f"Consider inlining critical CSS from: {asset['name']}")
        
        return list(set(suggestions))

    def print_analysis_results(self, page_type: str, metrics: Dict, slow_assets: List[Dict], suggestions: List[str]) -> None:
        """Print detailed analysis results to console"""
        print(f"\n{'='*50}")
        print(f"{page_type.upper()} PAGE ANALYSIS")
        print(f"{'='*50}")
        print(f"Total Load Time: {metrics['total_load_time']:.2f} seconds")
        
        print("\nTiming Breakdown:")
        for key, value in metrics['navigation_timing'].items():
            print(f"  {key.replace('_', ' ').title()}: {value}ms")
        
        if slow_assets:
            print("\nSlow Assets:")
            for asset in slow_assets:
                print(f"  - {asset['name']}")
                print(f"    Duration: {asset['duration']:.2f}ms")
                print(f"    Size: {asset['size']/1024:.1f}KB")
        
        if suggestions:
            print("\nOptimization Suggestions:")
            for suggestion in suggestions:
                print(f"  - {suggestion}")
        
        print(f"{'='*50}\n")


    def create_performance_graphs(self):
        """Create visualizations for performance metrics"""
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        
        game_load_times = self.metrics['game']['total_load_time']
        leaderboard_load_times = self.metrics['leaderboard']['total_load_time']
        min_len = min(len(game_load_times), len(leaderboard_load_times))

        # Adjust lists to have the same length
        game_load_times = game_load_times[:min_len]
        leaderboard_load_times = leaderboard_load_times[:min_len]
        iterations = range(1, min_len + 1)
        
        # Create a figure with multiple subplots
        plt.style.use('ggplot')  # Ensure valid style is applied
        fig = plt.figure(figsize=(15, 10))
        
        # 1. Load Time Comparison
        ax1 = plt.subplot(2, 2, 1)
        iterations = range(1, len(self.metrics['game']['total_load_time']) + 1)
        ax1.plot(iterations, self.metrics['game']['total_load_time'], 'b-o', label='Game')
        ax1.plot(iterations, self.metrics['leaderboard']['total_load_time'], 'r-o', label='Leaderboard')
        ax1.set_title('Page Load Time Comparison')
        ax1.set_xlabel('Iteration')
        ax1.set_ylabel('Load Time (seconds)')
        ax1.legend()
        ax1.grid(True)

        # 2. Average Timing Breakdown
        ax2 = plt.subplot(2, 2, 2)
        timing_categories = ['dns', 'tcp', 'request', 'response', 'dom_processing']
        game_timing_avg = {cat: sum(timing[cat] for timing in self.metrics['game']['asset_timings']) / len(self.metrics['game']['asset_timings']) 
                          for cat in timing_categories}
        leaderboard_timing_avg = {cat: sum(timing[cat] for timing in self.metrics['leaderboard']['asset_timings']) / len(self.metrics['leaderboard']['asset_timings']) 
                                 for cat in timing_categories}
        
        x = range(len(timing_categories))
        width = 0.35
        ax2.bar([i - width/2 for i in x], game_timing_avg.values(), width, label='Game')
        ax2.bar([i + width/2 for i in x], leaderboard_timing_avg.values(), width, label='Leaderboard')
        ax2.set_title('Average Timing Breakdown')
        ax2.set_xticks(x)
        ax2.set_xticklabels(timing_categories, rotation=45)
        ax2.set_ylabel('Time (ms)')
        ax2.legend()

        # 3. Resource Size Distribution
        ax3 = plt.subplot(2, 2, 3)
        game_sizes = [resource['size'] for resources in self.metrics['game']['resource_sizes'] 
                     for resource in resources]
        leaderboard_sizes = [resource['size'] for resources in self.metrics['leaderboard']['resource_sizes'] 
                            for resource in resources]
        
        sns.kdeplot(game_sizes, ax=ax3, label='Game')
        sns.kdeplot(leaderboard_sizes, ax=ax3, label='Leaderboard')
        ax3.set_title('Resource Size Distribution')
        ax3.set_xlabel('Size (bytes)')
        ax3.set_ylabel('Density')
        ax3.legend()

        plt.tight_layout()
        
        # Save the figure
        plt.savefig(self.reports_dir / f'performance_graphs_{timestamp}.png')
        plt.close()

    def generate_html_report(self):
        """Generate a comprehensive HTML report"""
        timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
        report_path = self.reports_dir / f'performance_report_{timestamp}.html'
        
        # Calculate statistics
        game_stats = {
            'avg_load_time': sum(self.metrics['game']['total_load_time']) / len(self.metrics['game']['total_load_time']),
            'min_load_time': min(self.metrics['game']['total_load_time']),
            'max_load_time': max(self.metrics['game']['total_load_time'])
        }
        
        leaderboard_stats = {
            'avg_load_time': sum(self.metrics['leaderboard']['total_load_time']) / len(self.metrics['leaderboard']['total_load_time']),
            'min_load_time': min(self.metrics['leaderboard']['total_load_time']),
            'max_load_time': max(self.metrics['leaderboard']['total_load_time'])
        }

        html_content = f"""
        <html>
        <head>
            <title>Performance Test Report - {timestamp}</title>
            <style>
                body {{ font-family: Arial, sans-serif; margin: 20px; }}
                .container {{ max-width: 1200px; margin: 0 auto; }}
                .stats-box {{ background: #f5f5f5; padding: 15px; margin: 10px 0; border-radius: 5px; }}
                .graph {{ margin: 20px 0; }}
                table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
                th, td {{ padding: 8px; border: 1px solid #ddd; text-align: left; }}
                th {{ background-color: #f5f5f5; }}
            </style>
        </head>
        <body>
            <div class="container">
                <h1>Performance Test Report</h1>
                <p>Generated on: {datetime.now().strftime("%Y-%m-%d %H:%M:%S")}</p>
                
                <h2>Summary Statistics</h2>
                <div class="stats-box">
                    <h3>Game Page</h3>
                    <p>Average Load Time: {game_stats['avg_load_time']:.2f} seconds</p>
                    <p>Min Load Time: {game_stats['min_load_time']:.2f} seconds</p>
                    <p>Max Load Time: {game_stats['max_load_time']:.2f} seconds</p>
                </div>
                
                <div class="stats-box">
                    <h3>Leaderboard Page</h3>
                    <p>Average Load Time: {leaderboard_stats['avg_load_time']:.2f} seconds</p>
                    <p>Min Load Time: {leaderboard_stats['min_load_time']:.2f} seconds</p>
                    <p>Max Load Time: {leaderboard_stats['max_load_time']:.2f} seconds</p>
                </div>

                <h2>Performance Graphs</h2>
                <div class="graph">
                    <img src="performance_graphs_{timestamp}.png" alt="Performance Graphs" style="max-width: 100%;">
                </div>
                
                <h2>Detailed Timing Data</h2>
                <table>
                    <tr>
                        <th>Iteration</th>
                        <th>Game Load Time</th>
                        <th>Leaderboard Load Time</th>
                        <th>Timestamp</th>
                    </tr>
        """
        
        for i in range(len(self.metrics['game']['total_load_time'])):
            html_content += f"""
                    <tr>
                        <td>{i + 1}</td>
                        <td>{self.metrics['game']['total_load_time'][i]:.2f}s</td>
                        <td>{self.metrics['leaderboard']['total_load_time'][i]:.2f}s</td>
                        <td>{self.metrics['game']['timestamp'][i].strftime("%Y-%m-%d %H:%M:%S")}</td>
                    </tr>
            """

        html_content += """
                </table>
            </div>
        </body>
        </html>
        """
        
        with open(report_path, 'w') as f:
            f.write(html_content)
        
        logging.info(f"HTML report generated: {report_path}")

    def run_test_suite(self, iterations: int = 10) -> None:
        """Run the complete test suite"""
        try:
            self.start_browser()
            print(f"\nStarting load time test suite - {iterations} iterations")
            
            for i in range(iterations):
                print(f"\nIteration {i+1}/{iterations}")
                
                # Test game page
                game_metrics = self.measure_page_load(self.game_url, 'game')
                if game_metrics:
                    slow_assets = self.analyze_slow_assets(game_metrics['resource_timing'])
                    suggestions = self.generate_optimization_suggestions(slow_assets)
                    self.print_analysis_results('game', game_metrics, slow_assets, suggestions)
                
                # Test leaderboard page
                leaderboard_metrics = self.measure_page_load(self.leaderboard_url, 'leaderboard')
                if leaderboard_metrics:
                    slow_assets = self.analyze_slow_assets(leaderboard_metrics['resource_timing'])
                    suggestions = self.generate_optimization_suggestions(slow_assets)
                    self.print_analysis_results('leaderboard', leaderboard_metrics, slow_assets, suggestions)
                
                time.sleep(2)  # Brief pause between iterations
            
            # Generate visualizations and report
            self.create_performance_graphs()
            self.generate_html_report()
            
        except Exception as e:
            logging.error(f"Error during test execution: {str(e)}")
        finally:
            if self.driver:
                self.driver.quit()

# Example usage
if __name__ == "__main__":
    test = GameLoadTimeTest(
        game_url="https://cs455-project.github.io/CS455-Assignment-1/",
        leaderboard_url="https://cs455-assignment-1.onrender.com/leaderboard"
    )
    test.run_test_suite(iterations=10)
