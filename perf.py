from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time
import statistics
import logging
import json
from datetime import datetime

class GamePerformanceTest:
    def __init__(self, url):
        self.url = url
        self.driver = None
        self.metrics = {
            'page_load_times': [],
            'fps_measurements': [],
            'memory_usage': [],
            'network_requests': []
        }
        self.setup_logging()

    def setup_logging(self):
        logging.basicConfig(
            filename=f'game_performance_{datetime.now().strftime("%Y%m%d_%H%M%S")}.log',
            level=logging.INFO,
            format='%(asctime)s - %(levelname)s - %(message)s'
        )

    def start_browser(self):
        options = webdriver.ChromeOptions()
        options.add_argument('--enable-precise-memory-info')
        options.add_argument('--js-flags="--expose-gc"')
        self.driver = webdriver.Chrome(options=options)
        
    def measure_page_load(self):
        start_time = time.time()
        self.driver.get(self.url)
        load_time = time.time() - start_time
        self.metrics['page_load_times'].append(load_time)
        logging.info(f"Page load time: {load_time:.2f} seconds")
        
    def measure_fps(self, duration=10):
        # Inject FPS measuring script with proper initialization
        fps_script = """
        window.fpsMonitor = {
            frameCount: 0,
            lastTime: performance.now(),
            fps: [],
            
            tick: function() {
                this.frameCount++;
                const now = performance.now();
                
                if (now - this.lastTime >= 1000) {
                    this.fps.push(this.frameCount);
                    this.frameCount = 0;
                    this.lastTime = now;
                }
                
                requestAnimationFrame(() => this.tick());
            },
            
            start: function() {
                this.frameCount = 0;
                this.lastTime = performance.now();
                this.fps = [];
                this.tick();
            },
            
            getFPS: function() {
                return this.fps;
            }
        };
        
        window.fpsMonitor.start();
        """
        
        # Initialize FPS monitoring
        self.driver.execute_script(fps_script)
        
        # Wait for the specified duration
        time.sleep(duration)
        
        # Collect FPS data
        fps_data = self.driver.execute_script("return window.fpsMonitor.getFPS();")
        
        if fps_data and len(fps_data) > 0:
            self.metrics['fps_measurements'].extend(fps_data)
            avg_fps = statistics.mean(fps_data)
            logging.info(f"Average FPS: {avg_fps:.2f}")
        else:
            logging.warning("No FPS data collected")

    def measure_memory(self):
        try:
            memory_info = self.driver.execute_script("""
                if (performance.memory) {
                    return {
                        jsHeapSize: performance.memory.usedJSHeapSize,
                        totalHeap: performance.memory.totalJSHeapSize
                    };
                }
                return null;
            """)
            
            if memory_info:
                self.metrics['memory_usage'].append(memory_info)
                logging.info(f"Memory usage: {memory_info['jsHeapSize'] / 1024 / 1024:.2f} MB")
            else:
                logging.warning("Memory metrics not available")
        except Exception as e:
            logging.error(f"Error measuring memory: {str(e)}")

    def capture_network_metrics(self):
        try:
            performance_timing = self.driver.execute_script("""
                return {
                    'network': performance.getEntriesByType('resource'),
                    'timing': performance.timing
                }
            """)
            self.metrics['network_requests'].append(performance_timing)
            logging.info("Network metrics captured successfully")
        except Exception as e:
            logging.error(f"Error capturing network metrics: {str(e)}")

    def run_test_suite(self, iterations=3):
        try:
            self.start_browser()
            logging.info(f"Starting performance test suite - {iterations} iterations")
            
            for i in range(iterations):
                logging.info(f"\nIteration {i+1}/{iterations}")
                self.measure_page_load()
                self.measure_fps()
                time.sleep(2)  # Give some time between measurements
                self.measure_memory()
                self.capture_network_metrics()
                
            self.generate_report()
            
        except Exception as e:
            logging.error(f"Error during test execution: {str(e)}")
        finally:
            if self.driver:
                self.driver.quit()

    def generate_report(self):
        report = {
            'timestamp': datetime.now().isoformat(),
            'summary': {
                'average_load_time': statistics.mean(self.metrics['page_load_times']) if self.metrics['page_load_times'] else 0,
                'average_fps': statistics.mean(self.metrics['fps_measurements']) if self.metrics['fps_measurements'] else 0,
                'memory_usage_avg': statistics.mean(
                    [m['jsHeapSize'] / 1024 / 1024 for m in self.metrics['memory_usage']]
                ) if self.metrics['memory_usage'] else 0
            },
            'detailed_metrics': self.metrics
        }
        
        filename = f'performance_report_{datetime.now().strftime("%Y%m%d_%H%M%S")}.json'
        with open(filename, 'w') as f:
            json.dump(report, f, indent=2)
        
        logging.info(f"Performance report generated successfully: {filename}")

# Example usage
if __name__ == "__main__":
    test = GamePerformanceTest("https://cs455-project.github.io/CS455-Assignment-1/")
    test.run_test_suite(iterations=3)